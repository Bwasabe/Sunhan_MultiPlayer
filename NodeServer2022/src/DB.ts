import MySQL, {RowDataPacket} from 'mysql2/promise'; // {}를 쓰고 안쓰고 차이 : Export Default냐 아니냐
// async await를 쓰게 해주는 녀석 : promise

const poolOption: MySQL.PoolOptions = {
    host: 'gondr.asuscomm.com',
    user: 'yy_40106',
    password: '1234',
    database: 'yy_40106',
    connectionLimit : 10
};

export const Pool: MySQL.Pool = MySQL.createPool(poolOption);

export interface ScoreVO extends RowDataPacket // C#의 인터페이스와 다르게 ts의 인터페이서는 클래스의 형태를 지정해준다
{
    id: number,
    score: number,
    username: string,
    time:Date
}

// export interface InventoryVO extends RowDataPacket // 내가 한 것
// {
//     id: number,
//     user_id: number,
//     json: string,
// }

export interface InventoryVO extends RowDataPacket // 선생님이 한 것
{
    id: number,
    user_id: number,
    json: string,
}

export interface InventoryVO extends RowDataPacket // 선생님이 한 것
{
    id: number,
    user_id: number,
    json: string,
}

export interface UserVO extends RowDataPacket
{
    id:number,
    account: string,
    name: string,
    pass:string
}

// export default class GGM
// {

// }

// export default class GGM2 -> 불가 : 즉 export default는 한 파일에서 하나만 존재
// {

// }