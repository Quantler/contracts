require('dotenv').load();

module.exports = {
  // See <http://truffleframework.com/docs/advanced/configuration>
  // to customize your Truffle configuration!
  networks : {
    development: {
      host: process.env.RPCHOST,
      port: 8545,
      network_id: "*" //Match any network id
    },
    rinkeby: {
      host: process.env.RPCHOST,
      port:8545,
      network_id: 4,
      gas: 6000000,
      from: process.env.RINKEBY_ACCOUNT
    }
  }
};
