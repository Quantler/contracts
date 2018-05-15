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
contract QuantTokenCrowdSale is TimedCrowdsale, IndividuallyCappedCrowdsale, MintedCrowdsale, PostDeliveryCrowdsale, AffiliateList {

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
    address public CompanyReserve;

    //Mining pool destination address
    address public MiningPool;

    //ICO bounty destination address
    address public ICOBounty;

    //Github bounty destination address
    address public GithubBounty; 

    //Company Percentage
    uint256 public CompanyPercentage;

    //MiningPool Percentage
    uint256 public MiningPoolPercentage;

    //ICOBounty Percentage
    uint256 public ICOBountyPercentage;

    //GithubBounty Percentage
    uint256 public GithubBountyPercentage;

    //Pre-Sale check
    bool public IsPreSaleOpen;
    
    //Helps us run a loop through addresses for token release
    address[] public addressIndices;

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
    uint256 _softcap,
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
      require(_wallet != address(0));
      require(_companyreserve != address(0));
      require(_miningpool != address(0));
      require(_icobounty != address(0));
      require(_githubbounty != address(0));
      require(_presalecap > 0);
      require(_presalerate > 0);
      require(_softcap > 0);

      //Set Caps and Rates
      HardCap = _cap;
      SoftCap = _softcap;
      PresaleCap = _presalecap;
      SoftCapRate = _softcaprate;
      HardCapRate = _hardcaprate;
      PresaleCapRate = _presalerate;

      //Set Wallets
      CompanyReserve = _companyreserve;
      MiningPool = _miningpool;
      ICOBounty = _icobounty;
      GithubBounty = _githubbounty; 

      //Set static items
      CompanyPercentage = 50;
      MiningPoolPercentage = 15;
      ICOBountyPercentage = 3;
      GithubBountyPercentage = 2;
  }

  /**
   * @dev Override to extend the way in which ether is converted to tokens.
   * @param _weiAmount Value in wei to be converted into tokens
   * @return Number of tokens that can be purchased with the specified _weiAmount
   */
  function _getTokenAmount(uint256 _weiAmount) internal view returns (uint256) {

    //Check currently active rate
    uint256 _rate = SoftCapRate;
    if(IsPreSaleOpen)
        _rate = PresaleCapRate;
    if(weiRaised >= SoftCap)
        _rate = HardCapRate;

    //Return total amount, based on current rate
    return _weiAmount.div(_rate);
  }

    /**
   * @dev Executed when a purchase has been validated and is ready to be executed. Not necessarily emits/sends tokens.
   * @param _beneficiary Address receiving the tokens
   * @param _tokenAmount Number of tokens to be purchased
   */
  function _processPurchase(address _beneficiary, uint256 _tokenAmount) internal {

    //TODO: overflow and overbought scenario! => refunds?

    //Check if additional affiliate amounts should be applied
    address affiliate = affiliateList[_beneficiary];
    uint256 totalTokens = _tokenAmount;
    if(affiliate != address(0)){
      uint256 affiliateTokens = _tokenAmount.mul(5).div(100);
      balances[affiliate]= balances[affiliate].add(affiliateTokens);
      _tokenAmount = _tokenAmount.mul(105).div(100);
      totalTokens = _tokenAmount.add(affiliateTokens);
      addressIndices.push(affiliate);
    }

    //Here we specify who gets what amount of tokens (Buyer/_beneficiary 30%, Company Address 50% etc...)
    balances[_beneficiary] = balances[_beneficiary].add(_tokenAmount);
    balances[CompanyReserve] = balances[CompanyReserve].add(totalTokens.mul(CompanyPercentage).div(30));
    balances[MiningPool] = balances[MiningPool].add(totalTokens.mul(MiningPoolPercentage).div(30));
    balances[ICOBounty] = balances[ICOBounty].add(totalTokens.mul(ICOBountyPercentage).div(30));
    balances[GithubBounty] = balances[GithubBounty].add(totalTokens.mul(GithubBountyPercentage).div(30));
    
    //We always need to add this address for indexing purpose
    addressIndices.push(_beneficiary);
  }

  /**
   * @dev Extend parent behavior requiring to be within contributing period
   * @param _beneficiary Token purchaser
   * @param _weiAmount Amount of wei contributed
   */
  function _preValidatePurchase(address _beneficiary, uint256 _weiAmount) internal {
    if(IsPreSaleOpen == false || weiRaised >= PresaleCap)
      super._preValidatePurchase(_beneficiary, _weiAmount);
  }

  /**
   * @dev Open the presale on any given moment
   */
  function openPresale() external onlyOwner{
    IsPreSaleOpen = true;
  }

  /**
   * @dev Close the presale manually
   */
  function closePresale() external onlyOwner{
    IsPreSaleOpen = false;
  }
  
  /**
   * @dev Withdraw all tokens after crowdsale ends.
   */
  function withdrawAllTokens() external onlyOwner {
    require(hasClosed());
    for(uint i = 0; i < addressIndices.length; i++){
        address recipient = addressIndices[i];
        uint256 amount = balances[msg.sender];
        if(amount > 0)
        {
            balances[recipient] = 0;
            _deliverTokens(recipient, amount);
        }
    }
  }
}