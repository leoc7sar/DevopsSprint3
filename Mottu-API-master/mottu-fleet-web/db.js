const sql = require('mssql');
const config = { connectionString: process.env.SQL_CONNECTION };
let poolPromise;
async function getPool(){ if(!poolPromise){ poolPromise = sql.connect(config.connectionString); } return poolPromise; }
module.exports = { sql, getPool };
