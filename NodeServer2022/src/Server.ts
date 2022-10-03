import http from 'http'; // 이렇게 쓰면 ts가 알아서 require로 변경해준다.
// const http = require('http');
import Express, {Application, Request, Response} from 'express';
// const Express = require('express');
import path from 'path';
import fs from 'fs';

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
    console.log(fileList);
    
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