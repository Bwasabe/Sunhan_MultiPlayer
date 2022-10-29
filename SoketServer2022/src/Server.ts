import Express, { Application } from 'express';
import {IncomingMessage} from 'http'
import WS from 'ws';

const App: Application = Express();

const httpServer = App.listen(50000, () => {
   console.log("Server is running on 50000 port");
    
});

const soketServer: WS.Server = new WS.Server({
    server: httpServer,
    // port:50000
}, () => {
    console.log("Socket server is running on 50000 port");
});

soketServer.on("connection", (soc: WS, req: IncomingMessage) => {

    soc.send("Welcome to My Server!");
    soc.send("Welcome to My Server222!");
});