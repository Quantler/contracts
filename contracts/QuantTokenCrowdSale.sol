pragma solidity ^0.4.17;
import "./QuantToken.sol";
import "./AffiliateList.sol";

import "zeppelin-solidity/contracts/crowdsale/validation/IndividuallyCappedCrowdsale.sol";
import "zeppelin-solidity/contracts/crowdsale/validation/TimedCrowdsale.sol";
import "zeppelin-solidity/contracts/crowdsale/emission/MintedCrowdsale.sol";
import "zeppelin-solidity/contracts/crowdsale/distribution/PostDeliveryCrowdsale.sol";

/**
 * @title SampleCrowdsale
 * @dev General crowd sale of the QUANT Token, following integrations is used:
 *      - IndividuallyCappedCrowdsale: used for whitelisting (individually capped), if not present you cannot buy any tokens
 *      - TimedCrowdsale: CrowdSale will last a specific length
 *      - MintedCrowdsale: Tokens will be minted
 *      - PostDeliveryCrowdsale: Tokens will be distributed after crowdsale
 *      - AffiliateList: affiliate distribution and registration
 */
contract QuantTokenCrowdSale is TimedCrowdsale, IndividuallyCappedCrowdsale, MintedCrowdsale, PostDeliveryCrowdsale {

    // Current public soft cap
    uint256 public SoftCap;

    // Current public hard cap
    uint256 public HardCap;

    // Current public presale cap
    uint256 public PresaleCap;

    // Current public soft cap-rate
    uint256 public SoftCapRate;

    // Current public hard cap-rate
    uint256 public HardCapRate;

    // Current public presale cap-rate
    uint256 public PresaleCapRate;

    //Company reserve destionation address
    address CompanyReserve;

    //Mining pool destination address
    address MiningPool;

    //ICO bounty destination address
    address ICOBounty;

    //Github bounty destination address
    address GithubBounty; 

    //Company Percentage
    uint256 CompanyPercentage;

    //MiningPool Percentage
    uint256 MiningPoolPercentage;

    //ICOBounty Percentage
    uint256 ICOBountyPercentage;

    //GithubBounty Percentage
    uint256 GithubBountyPercentage;

    //Affiliate registry
    AffiliateList public affiliateList;

    //Pre-Sale check
    bool IsPreSaleOpen;

  /**
   * @dev The QuantTokenCrowdSale constructor sets the initial crowdsale parameters.
   */
  function QuantTokenCrowdSale(
    uint256 _openingTime,
    uint256 _closingTime,
    uint256 _softcaprate,
    uint256 _hardcaprate,
    uint256 _presalerate,
    address _wallet,
    address _companyreserve,
    address _miningpool,
    address _icobounty,
    address _githubbounty,
    uint256 _cap,
    uint256 _softcaperc,
    uint256 _presalecap,
    QuantToken _token
  )
    public
    Crowdsale(_softcaprate, _wallet, _token)
    TimedCrowdsale(_openingTime, _closingTime)
  {
      //Check input
      require(_cap > 0);
      require(_openingTime > 0);
      require(_closingTime > 0);
      require(_softcaprate > 0);
      require(_softcaprate > _hardcaprate);
      require(_softcaprate > _hardcaprate);
      require(_wallet != address(0));
      require(_companyreserve != address(0));
      require(_miningpool != address(0));
      require(_icobounty != address(0));
      require(_githubbounty != address(0));
      require(_presalecap > 0);
      require(_presalerate > 0);

      //Set items
      HardCap = _cap;
      SoftCap = HardCap.mul(_softcaperc);
      PresaleCap = _presalecap;
      SoftCapRate = _softcaprate;
      HardCapRate = _hardcaprate;
      PresaleCapRate = _presalerate;
      affiliateList = new AffiliateList();
  }

  /**
   * @dev Override to extend the way in which ether is converted to tokens.
   * @param _weiAmount Value in wei to be converted into tokens
   * @return Number of tokens that can be purchased with the specified _weiAmount
   */
  function _getTokenAmount(uint256 _weiAmount) internal view returns (uint256) {
    
    //Check if soft-cap has been hit
    uint256 _rate = SoftCapRate;
    if(weiRaised > SoftCap)
        _rate = HardCapRate;

    //Check if additional affiliate amounts should be applied
    //if(affiliateList.getAffiliate(0))
    if(1==1)
        _rate = _rate.mul(110).div(100);

    //Return total amount, based on current rate
    return _weiAmount.mul(_rate);
  }

    /**
   * @dev Executed when a purchase has been validated and is ready to be executed. Not necessarily emits/sends tokens.
   * @param _beneficiary Address receiving the tokens
   * @param _tokenAmount Number of tokens to be purchased
   */
  function _processPurchase(address _beneficiary, uint256 _tokenAmount) internal {
      //Here we specify who gets what amount of tokens (Buyer/_beneficiary 30%, Company Address 50% etc...) + affiliate
    balances[_beneficiary] = balances[_beneficiary].add(_tokenAmount);
  }

  /**
   * @dev Extend parent behavior requiring to be within contributing period
   * @param _beneficiary Token purchaser
   * @param _weiAmount Amount of wei contributed
   */
  function _preValidatePurchase(address _beneficiary, uint256 _weiAmount) internal {
    if(IsPreSaleOpen == false)
      super._preValidatePurchase(_beneficiary, _weiAmount);
    else
      IsPreSaleOpen = weiRaised <= PresaleCap;
  }

  /**
   * @dev Open the presale on any given moment
   */
  function _openPresale() external onlyOwner{
    IsPreSaleOpen = true;
  }

  /**
   * @dev Close the presale manually
   */
  function _closePresale() external onlyOwner{
    IsPreSaleOpen = false;
  }
}