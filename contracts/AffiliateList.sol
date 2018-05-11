pragma solidity ^0.4.17;
import "./Curatable.sol";

/**
 * @title AffiliateList
 * @dev Keeps track of affiliate links between investor and affiliates.
 */
contract AffiliateList is Curatable {

    mapping (address => address) public affiliateList;

  /**
   * @dev Links an affiliate with a specific investor
   * @param _affiliate Address of the affiliate
   * @param _investor Address of the investor
   */
    function addAffiliate(address _affiliate, address _investor) external onlyCurator {
        require(_investor != 0x0 && _affiliate != 0x0);
        require(_affiliate != _investor);
        affiliateList[_investor] = _affiliate;
    }


  /**
   * @dev Get the current link between affiliate and investor, if it exists
   * @param _investor Address of the investor
   */
    function getAffiliate(address _investor) external returns (address affiliate) {
        return affiliateList[_investor];
    }

  /**
   * @dev Links an affiliate with a specific group of investor
   * @param _affiliate Address of the affiliate
   * @param _investors Addresses of the investors
   */
    function setGroupAffiliate(address _affiliate, address[] _investors) external onlyCurator{
        for (uint256 i = 0; i < _investors.length; i++) {
            affiliateList[_investors[i]] = _affiliate;
        }
    }

}