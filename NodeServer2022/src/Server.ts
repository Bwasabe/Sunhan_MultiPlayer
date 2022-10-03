import http from 'http'; // 이렇게 쓰면 ts가 알아서 require로 변경해준다.
// const http = require('http');
import Express from 'express';
// const Express = require('express');


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

app.get("/", (req, res) => {
    res.json(data);
});

app.get("/image", (req, res) => {
    // 아직 미구현
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