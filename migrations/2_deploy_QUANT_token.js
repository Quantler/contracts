const QuantTokenSetup = artifacts.require('./QuantTokenCrowdSale.sol')
module.exports = function(deployer) {
    deployer.deploy(QuantTokenSetup);
}