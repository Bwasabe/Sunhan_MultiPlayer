let http = require('http');

let a = () => {
    
};


let server = http.createServer((req, res) => {

    console.log(req.url);

    switch (req.url)
    {
        case "/":
            res.end("Main Page");
            break;
        case "/image":
            res.end("Image Page");
            break;
        default:
            res.end("Not exist!");
            break;
    }

    //res.end("Hello World"); // end안에 있는 매개변수를 전달
});


server.listen(50000, () => {
    console.log("서버가 50000번 포트에서 실행중입니다.");
});

// let a = 10;
// let b = 20;
// let c = a + b;
// console.log(c); // Debug.Log()와 동일