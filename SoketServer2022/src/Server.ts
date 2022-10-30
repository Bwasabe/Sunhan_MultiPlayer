import Express, { Application } from 'express';
import {IncomingMessage} from 'http'
import WS, { RawData } from 'ws';
import { tankio } from './packet'; 

const App: Application = Express();

const httpServer = App.listen(50000, () => {
   console.log("Server is running on 50000 port");
    
});

const soketServer: WS.Server = new WS.Server({
    server: httpServer,
    // port:50000
});

soketServer.on("connection", (soc: WS, req: IncomingMessage) => {

    soc.on('message', (data: RawData, isBinary: boolean) => {
        console.log(isBinary);
        console.log(data);
        
        let length: number = (data.slice(0, 2) as Buffer).readInt16LE();
        let code: number = (data.slice(2, 4) as Buffer).readInt16LE();   // 16비트 정수를 읽어(ushort) - Little Endian
        let payload: Buffer = data.slice(4) as Buffer; // 4부터 끝까지 잘라내기
        
        if (code == 0) {
            console.log("CMove message 도착");
            let cMove: tankio.CMove = tankio.CMove.deserialize(payload);
            console.log(cMove.playerId, cMove.x, cMove.y);
        }

        // let lenBuffer: Buffer = data.slice(0, 2) as Buffer;
        // let length = lenBuffer.readInt16LE();   // 16비트 정수를 읽어(ushort) - Little Endian
        // let typeBuffer: Buffer = data.slice(2, 4) as Buffer;
        // let type = typeBuffer.readInt16LE();
        // let aBuf: Buffer = data.slice(4, 8) as Buffer;
        // let bBuf: Buffer = data.slice(8, 12) as Buffer;
        // let a;
        // let b;
        // if (type == 1) // SendData
        // {
        //     a = aBuf.readInt32LE();
        //     b = bBuf.readInt32LE();
        // }
        // else if (type == 2) {
        //     a = aBuf.readFloatLE();
        //     b = bBuf.readFloatLE();
        // }
        // console.log(length);
        // console.log(type);
        // console.log(a);
        // console.log(b);
    });
    // soc.send("Welcome to My Server!");
    // soc.send("Welcome to My Server222!");
});