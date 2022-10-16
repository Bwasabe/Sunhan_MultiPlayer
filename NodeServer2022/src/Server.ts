import http from 'http'; // 이렇게 쓰면 ts가 알아서 require로 변경해준다.
// const http = require('http');
import Express, {Application, Request, Response} from 'express';
// const Express = require('express');
import path from 'path';
import fs from 'fs';

import { InventoryVO, Pool, ScoreVO, UserVO } from './DB';
import { FieldPacket, ResultSetHeader } from 'mysql2';

import JWT from 'jsonwebtoken';
import { Key } from './Secret';

// import GGM1 from './DB';

// 깃에는 노드 모듈이 필요가 없기 때문에 깃 이그노어에 노드 모듈이 들어감, 나중에 받은 후 npm i 하나만 치면 알아서 패키지가 설치됨


const app: Express.Application = Express();
app.use(Express.json()); // 들어오는 post 데이터를 json으로 변경해서 body에 박아주는 역할
app.use(Express.urlencoded({ extended: true })); // 한글때문에 적어줌


app.get("/", (req : Request, res : Response) => {
    // res.json(data);
});

app.post("/login", async (req: Request, res: Response) => {
    const { account, pass }: { account: string, pass: string } = req.body;
    
    console.log(account, pass);
    // TODO : 여기서 로그인처리를 어케어케 하고 나서

    // TODO: 로그인을 성공시에는 토큰 발행, 실패시에는 토큰 미발행(실패했다고 보내줌)


    let token:string = JWT.sign({
        //Payload
        account: account, 
        name:"최선한"
    }, Key.secret, {
        algorithm: "HS256",
        expiresIn: "30 days"
    });
    
    res.json({ msg: "로그인 성공", token: token }); // 키와 변수명이 똑같을 경우 생략 가능 account,

});


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
// app.post("/insert/Inventory", async (req: Request, res: Response) => {
//     console.log(req.body);
//     const { user_id, json } = req.body;

//     let [result, info] : [ResultSetHeader, FieldPacket[]] = await Pool.query(`INSERT INTO Inventories (user_id, json) VALUES (?,?)`, [user_id, json]);

//     res.json({ msg: "기록 완료!", user_id: result.insertId });
// });

// 세이브
app.post("/inven", async (req: Request, res: Response) => {
    const { user_id, json }: { user_id: number, json: string } = req.body; // 구조분해 할당 + 타입 지정
    let sql = `SELECT * FROM Inventories WHERE user_id = ?`; // 테이블명은 대소문자 구분한다
    let [row, fieldInfos]: [InventoryVO[], FieldPacket[]] = await Pool.query(sql, [user_id]);
    
    try
    {
        if (row.length == 0) // 처음으로 데이터베이스에 들어갈 경우
        {
            sql = `INSERT INTO Inventories (user_id, json) VALUES (?, ?)`;
            let [result, info]: [ResultSetHeader, FieldPacket[]] = await Pool.query(sql, [user_id, json]);
            if (result.affectedRows != 1) throw `Not Affected`;
        }
        else
        {
            sql = `UPDATE Inventories SET json = ? WHERE user_id = ?`;
            let [result, info]: [ResultSetHeader, FieldPacket[]] = await Pool.query(sql, [json, user_id]);
            if (result.affectedRows != 1) throw `Not Affected`;

        }
        res.json({ success: true, msg: "저장 완료" }); // 원래는 이것도 따로 타입을 만들어 주는게 좋다

    } catch (e){
        console.log(e);
        res.json({ success: false, msg: "데이터베이스 저장 중 오류 발생" });
    }
});

app.post("/user", async (req: Request, res: Response) => {
    let sql = `INSERT INTO users (account, name, pass) VALUES(?,?,PASSWORD(?))`;
    const { account, name, pass }: { account: string, name: string, pass: string } = req.body;

    try {
        let [result, info]: [ResultSetHeader, FieldPacket[]] = await Pool.query(sql, [account, name, pass]);
        if (result.affectedRows != 1) throw "Errow";
        res.json({ success: true, msg: "성공적으로 회원가입" });
    } catch (e) {
        console.log(e);
        res.json({ success: false, msg: "회원가입 중 오류 발생" });
    }

    
});

// 로드
app.get("/inven", async (req: Request, res: Response) => {
    const user_id : number = 1; // 이건 나중에 토큰에서 값을 빼서 인증하는 시긍로 변경될꺼다
    let sql = `SELECT * FROM Inventories WHERE user_id = ?`;
    let [rows, fieldInfos]: [InventoryVO[], FieldPacket[]] = await Pool.query(sql, [user_id]);

    if (rows.length == 0)
    {
        res.json({msg: "로딩완료", data :""});   
    } else {
        res.json({msg: "로딩완료", data : rows[0].json});   
    }
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

// app.get("/get/Inventory", async (req: Request, res: Response) => {
    
//     const sql = `SELECT * FROM Inventories ORDER BY id ASC`;
//     let [rows, fieldInfos] : [InventoryVO[], FieldPacket[]] = await Pool.query(sql);
    
//     console.log(rows[0]);
//     // res.send(rows);
//     res.json({ msg: "인벤토리정보 보냄", json: JSON.stringify(rows[0])});
// });



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



// // JavaScript Object Notation(JSON)
// const data = {
//     name: "방과후 데이터",
//     users: [
//         { id: 1, name: "김동윤" },
//         { id: 2, name: "김대현" },
//         { id: 3, name: "유하준" },
//     ]
// };
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