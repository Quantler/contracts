pragma solidity ^0.4.17;

import "zeppelin-solidity/contracts/token/ERC20/MintableToken.sol";

/**
 * @title QuantToken
 * @dev The QUANT Token implementation
 */
contract QuantToken is MintableToken {

  string public constant name = "QuantToken"; // solium-disable-line uppercase
  string public constant symbol = "QUANT"; // solium-disable-line uppercase
  uint256 public constant decimals = 18; // solium-disable-line uppercase

}
