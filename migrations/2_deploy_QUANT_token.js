const QuantTokenCrowdSaleSetup = artifacts.require('./QuantTokenCrowdSale.sol')
const QuantTokenSetup = artifacts.require('./QuantToken.sol')


module.exports = function(deployer, network, accounts) {
    deployer.deploy(QuantTokenSetup).then(async function(){
        await deployer.deploy(QuantTokenCrowdSaleSetup);
    });
}