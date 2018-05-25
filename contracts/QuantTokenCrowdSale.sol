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

    //Currently amount of tokens allocated
    uint256 public TokensAllocated;

    //Helps us run a loop through addresses for token release
    address[] public addressIndices;

    //If true, all tokens have been minted and released (end of ICO)
    bool public IsTokensReleased;

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
    Crowdsale(_hardcaprate, _wallet, _token)
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
      IsTokensReleased = false;
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
    else if(weiRaised >= SoftCap)
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

    //Check if additional affiliate amounts should be applied
    address affiliate = affiliateList[_beneficiary];
    uint256 totalTokens = _tokenAmount;
    if(affiliate != address(0)){
      uint256 affiliateTokens = _tokenAmount.mul(5).div(100);
      balances[affiliate]= balances[affiliate].add(affiliateTokens);
      _tokenAmount = _tokenAmount.mul(105).div(100);
      totalTokens = _tokenAmount.add(affiliateTokens);
      _addKnownAddress(affiliate);
    }

    //Check if we need to cut down on token amount since we might have already hit the cap on tokens sold
    uint256 newTokenAmount = TokensAllocated.add(totalTokens);
    if(newTokenAmount >= 54000000){ //Overflow HardCap
      uint256 toBeRefunded = newTokenAmount.sub(54000000);
      _beneficiary.transfer(toBeRefunded.mul(HardCapRate));
      _tokenAmount = _tokenAmount.sub(toBeRefunded);
    }

    //Set balance before token release
    balances[_beneficiary] = balances[_beneficiary].add(_tokenAmount);
    TokensAllocated = TokensAllocated.add(totalTokens);
    
    //We always need to add this address for indexing purpose
    _addKnownAddress(_beneficiary);
  }

  /**
   * @dev Extend parent behavior requiring to be within contributing period
   * @param _beneficiary Token purchaser
   * @param _weiAmount Amount of wei contributed
   */
  function _preValidatePurchase(address _beneficiary, uint256 _weiAmount) internal {
    if(IsPreSaleOpen == false || weiRaised >= PresaleCap)
      super._preValidatePurchase(_beneficiary, _weiAmount);
    else
      require(contributions[_beneficiary].add(_weiAmount) <= caps[_beneficiary]);
  }

  /**
   * @dev Add this address as known address
   * @param _address Address to add
   */
  function _addKnownAddress(address _address) internal{
        bool found = false;
        for(uint i = 0; i < addressIndices.length; i++){
          if(address(addressIndices[i]) == _address)
            found = true;
        }
        if(!found)
            addressIndices.push(_address);
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
   * @dev Withdraw all tokens after crowdsale ends. (can only be done once and only after sale has ended!)
   */
  function withdrawAllTokens() external onlyOwner {
    //Check input
    require(hasClosed());
    require(!IsTokensReleased);

    //Allocate tokens to sub accounts
    uint256 allocationKey = TokensAllocated.mul(100).div(30);
    balances[CompanyReserve] = allocationKey.mul(CompanyPercentage).div(100);
    balances[MiningPool] = allocationKey.mul(MiningPoolPercentage).div(100);
    balances[ICOBounty] = allocationKey.mul(ICOBountyPercentage).div(100);
    balances[GithubBounty] = allocationKey.mul(GithubBountyPercentage).div(100);

    //Check for overflow via GithubBounty and correct if this is the case
    TokensAllocated = allocationKey + balances[CompanyReserve] + balances[MiningPool] + balances[ICOBounty] + balances[GithubBounty];
    uint256 maxTokens = 180000000;
    if(TokensAllocated > maxTokens)
      balances[GithubBounty] = balances[GithubBounty] - (TokensAllocated - maxTokens);

    //Add addresses to known addresses
    _addKnownAddress(CompanyReserve);
    _addKnownAddress(MiningPool);
    _addKnownAddress(ICOBounty);
    _addKnownAddress(GithubBounty);

    //Mint and send all tokens
    for(uint i = 0; i < addressIndices.length; i++){
        address recipient = addressIndices[i];
        uint256 amount = balances[recipient];
        if(amount > 0)
        {
            balances[recipient] = 0;
            _deliverTokens(recipient, amount);
        }
    }

    //Mark End of All
    IsTokensReleased = true;
  }
}