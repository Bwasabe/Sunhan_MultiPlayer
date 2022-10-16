import http from 'http'; // 이렇게 쓰면 ts가 알아서 require로 변경해준다.
// const http = require('http');
import Express, {Application, Request, Response} from 'express';
// const Express = require('express');
import path from 'path';
import fs from 'fs';

import { InventoryVO, Pool, ScoreVO } from './DB';
import { FieldPacket, ResultSetHeader } from 'mysql2';

// import GGM1 from './DB';

// 깃에는 노드 모듈이 필요가 없기 때문에 깃 이그노어에 노드 모듈이 들어감, 나중에 받은 후 npm i 하나만 치면 알아서 패키지가 설치됨

// JavaScript Object Notation(JSON)
const data = {
    name: "방과후 데이터",
    users: [
        { id: 1, name: "김동윤" },
        { id: 2, name: "김대현" },
        { id: 3, name: "유하준" },
    ]
};

const app: Express.Application = Express();

app.get("/", (req : Request, res : Response) => {
    res.json(data);
});

app.use(Express.json()); // 들어오는 post 데이터를 json으로 변경해서 body에 박아주는 역할
app.use(Express.urlencoded({ extended: true })); // 한글때문에 적어줌

/*
ResultSetHeader {
    fieldCount: 0,
    affectedRows: 1, // 영향을 받는 열
    insertId: 6, // AUTO_INCREMENT 
    info: '',
    serverStatus: 2,
    warningStatus: 0
  },
*/
app.post("/insert/Inventory", async (req: Request, res: Response) => {
    console.log(req.body);
    const { user_id, json } = req.body;

    let [result, info] : [ResultSetHeader, FieldPacket[]] = await Pool.query(`INSERT INTO Inventories (user_id, json) VALUES (?,?)`, [user_id, json]);

    res.json({ msg: "기록 완료!", user_id: result.insertId });
});
app.post("/insert", async (req: Request, res: Response) => {
    const { score, username } = req.body;

    console.log(req.body);
    
    // 싱글스레드 언어
    let [result, info] : [ResultSetHeader, FieldPacket[]] = await Pool.query(`INSERT INTO scores (score,username, time) VALUES (?,?, NOW())`, [score, username]);
    
    // console.log(result);

    // 기록된 id도 함께 리턴하게 해서 유니티에서 해당 ID(Auto_IncreamentID)를 출력하게 하기
    res.json({ msg: "성공적으로 기록 완료" , incrementID : result.insertId});
});

app.get("/get/Inventory", async (req: Request, res: Response) => {
    
    const sql = `SELECT * FROM Inventories ORDER BY id ASC`;
    let [rows, fieldInfos] : [InventoryVO[], FieldPacket[]] = await Pool.query(sql);
    
    console.log(rows);
    // res.send(rows);
    res.json({ msg: "인벤토리정보 보냄", json: rows[0] });
});



app.get("/record", async (req: Request, res: Response) => {
    const sql = `SELECT * FROM scores ORDER BY score DESC LIMIT 0, 5`;
    
    let [rows, fieldInfos] : [ScoreVO[], FieldPacket[]] = await Pool.query(sql);
    // console.log(result[0]); // 각 필드들을 말함
    // console.log(result[1]); // 각 필드들의 자료형 같은 정보들을 가져옴
    
    res.json({ msg: 'data list', count: rows.length, data: rows });
});

app.get("/image/:filename", (req: Request, res: Response) => {
    let filename: string = req.params.filename;
    let filePath = path.join(__dirname, "..", "images", filename);
    if (!fs.existsSync(filePath))
    {
        filePath = path.join(__dirname, "..", "images", "안아줘요 태영.png");
    }
    res.sendFile(filePath);
    
});

app.get("/imagelist", (req: Request, res: Response) => {
    let imagePath = path.join(__dirname, "..", "images");
    let fileList: string[] = fs.readdirSync(imagePath);
    fileList = fileList.filter(x => x != "안아줘요 태영.png");
    // console.log(fileList);
    
    let msg = {
        text: "성공적으로 로딩",
        count: fileList.length,
        list: fileList
    }

    res.json(msg);
});

const server = http.createServer(app);


server.listen(50000, () => {
    console.log("서버가 50000번 포트에서 실행중입니다.");
});


// const server = http.createServer((req, res) => {
//     // console.log(JSON.stringify(data));
//     switch (req.url)
//     {
//         case "/":
//             let msg = JSON.stringify(data);
//             res.writeHead(200, { "Content-Type": "application/json" });
//             res.end(msg);
//             break;
//         case "/image":
//             res.end("Image Page");
//             break;
//         default:
//             res.end("Not exist!");
//             break;
//     }
//     //res.end("Hello World"); // end안에 있는 매개변수를 전달
// });


// let a = 10;
// let b = 20;
// let c = a + b;
// let a = () => {
// };
// console.log(c); // Debug.Log()와 동일