# Abstraction

> **"Abstraction is the process of hiding implementation details while showing only essential features to the user. It focuses on what an object does rather than how it does it."**

## 🎯 Definition

Abstraction is the OOP principle that focuses on **hiding complexity** and showing only **essential functionality** to the user. It creates a simplified interface that represents the most important aspects of an object while concealing the intricate implementation details.

## 🏠 Real-World Analogy

Think of **driving a car**:

- ✅ **Simple interface** - Steering wheel, pedals, gear shift, dashboard
- ✅ **Hidden complexity** - Engine internals, transmission mechanics, fuel injection
- ✅ **Essential operations exposed** - Start, stop, accelerate, brake, turn
- ✅ **Implementation irrelevant** - Electric vs gasoline engine doesn't change driving interface
- ✅ **Different implementations** - Same driving interface works for different car models

## 📊 UML Diagram

```mermaid
classDiagram
    class IPaymentProcessor {
        <<interface>>
        +ProcessPayment(amount: decimal, paymentDetails: PaymentDetails) : PaymentResult
        +ValidatePayment(paymentDetails: PaymentDetails) : bool
        +GetSupportedMethods() : List~string~
    }
    
    class CreditCardProcessor {
        -apiClient: ICreditCardApi
        -encryptionService: IEncryptionService
        -fraudDetection: IFraudDetection
        
        +ProcessPayment(amount: decimal, paymentDetails: PaymentDetails) : PaymentResult
        +ValidatePayment(paymentDetails: PaymentDetails) : bool
        +GetSupportedMethods() : List~string~
        
        -ValidateCreditCard(cardNumber: string) : bool
        -EncryptCardData(cardData: CreditCardData) : string
        -CheckFraudRisk(transaction: Transaction) : RiskLevel
        -CallBankApi(request: PaymentRequest) : ApiResponse
    }
    
    class PayPalProcessor {
        -paypalApi: IPayPalApi
        -tokenService: ITokenService
        
        +ProcessPayment(amount: decimal, paymentDetails: PaymentDetails) : PaymentResult
        +ValidatePayment(paymentDetails: PaymentDetails) : bool
        +GetSupportedMethods() : List~string~
        
        -AuthenticateWithPayPal(credentials: PayPalCredentials) : AuthToken
        -CreatePayPalTransaction(request: PayPalRequest) : string
        -ConfirmPayment(transactionId: string) : bool
    }
    
    class BankTransferProcessor {
        -bankingService: IBankingService
        -complianceChecker: IComplianceChecker
        
        +ProcessPayment(amount: decimal, paymentDetails: PaymentDetails) : PaymentResult
        +ValidatePayment(paymentDetails: PaymentDetails) : bool
        +GetSupportedMethods() : List~string~
        
        -ValidateAccountNumber(accountNumber: string) : bool
        -CheckComplianceRules(transfer: BankTransfer) : ComplianceResult
        -InitiateTransfer(transferRequest: TransferRequest) : TransferResult
    }
    
    IPaymentProcessor <|.. CreditCardProcessor : implements
    IPaymentProcessor <|.. PayPalProcessor : implements
    IPaymentProcessor <|.. BankTransferProcessor : implements
    
    note for IPaymentProcessor : "Abstract interface hides\nimplementation complexity.\nClients only see essential\noperations."
    
    note for CreditCardProcessor : "Concrete implementation\nhandles credit card specifics:\n- Encryption\n- Fraud detection\n- Bank API calls"
```

## 🚫 Violation Example (No Abstraction)

```csharp
// ❌ BAD: No abstraction - clients must know implementation details

public class OrderService
{
    // Client must know about all payment implementation details!
    public async Task<bool> ProcessOrderPayment(Order order, string paymentMethod, PaymentData paymentData)
    {
        try
        {
            if (paymentMethod == "creditcard")
            {
                // Credit card specific logic exposed to client
                var creditCardApi = new CreditCardApiClient("https://api.creditcard.com", "api-key-123");
                
                // Client must know validation rules
                if (paymentData.CardNumber.Length != 16)
                    return false;
                
                if (paymentData.ExpiryDate < DateTime.Now)
                    return false;
                
                // Client must handle encryption
                var encryptionService = new EncryptionService();
                var encryptedCard = encryptionService.EncryptCardNumber(paymentData.CardNumber);
                var encryptedCvv = encryptionService.EncryptCvv(paymentData.Cvv);
                
                // Client must know fraud detection
                var fraudDetector = new FraudDetectionService();
                var riskScore = await fraudDetector.CalculateRiskScore(paymentData.CardNumber, order.Amount);
                if (riskScore > 0.8)
                    return false;
                
                // Client must construct API request
                var request = new CreditCardPaymentRequest
                {
                    Amount = order.Amount,
                    Currency = "USD",
                    CardNumber = encryptedCard,
                    Cvv = encryptedCvv,
                    ExpiryMonth = paymentData.ExpiryDate.Month,
                    ExpiryYear = paymentData.ExpiryDate.Year,
                    MerchantId = "MERCH123",
                    TransactionId = Guid.NewGuid().ToString()
                };
                
                // Client must handle API communication
                var response = await creditCardApi.ProcessPaymentAsync(request);
                
                // Client must interpret response codes
                if (response.StatusCode == "00") // Success
                {
                    order.PaymentConfirmationCode = response.AuthorizationCode;
                    return true;
                }
                else if (response.StatusCode == "05") // Declined
                {
                    order.PaymentErrorMessage = "Card declined";
                    return false;
                }
                // ... many more status codes to handle
            }
            else if (paymentMethod == "paypal")
            {
                // PayPal specific logic - completely different implementation!
                var paypalApi = new PayPalApiClient();
                
                // Different authentication mechanism
                var authToken = await paypalApi.GetAuthTokenAsync("client-id", "client-secret");
                
                // Different validation rules
                if (string.IsNullOrEmpty(paymentData.PayPalEmail))
                    return false;
                
                if (!IsValidEmail(paymentData.PayPalEmail))
                    return false;
                
                // Different request structure
                var paypalRequest = new PayPalPaymentRequest
                {
                    Intent = "sale",
                    Payer = new PayPalPayer
                    {
                        PaymentMethod = "paypal",
                        PayerInfo = new PayPalPayerInfo
                        {
                            Email = paymentData.PayPalEmail
                        }
                    },
                    Transactions = new[]
                    {
                        new PayPalTransaction
                        {
                            Amount = new PayPalAmount
                            {
                                Total = order.Amount.ToString("F2"),
                                Currency = "USD"
                            },
                            Description = $"Order #{order.Id}"
                        }
                    },
                    RedirectUrls = new PayPalRedirectUrls
                    {
                        ReturnUrl = "https://oursite.com/paypal/success",
                        CancelUrl = "https://oursite.com/paypal/cancel"
                    }
                };
                
                // Different API call
                var paypalResponse = await paypalApi.CreatePaymentAsync(paypalRequest, authToken);
                
                // Different response handling
                if (paypalResponse.State == "created")
                {
                    // PayPal requires additional approval step
                    var approvalUrl = paypalResponse.Links.FirstOrDefault(l => l.Rel == "approval_url")?.Href;
                    order.PaymentApprovalUrl = approvalUrl;
                    return true;
                }
            }
            else if (paymentMethod == "banktransfer")
            {
                // Bank transfer logic - yet another completely different implementation!
                var bankingService = new BankingService();
                
                // Different validation
                if (!IsValidAccountNumber(paymentData.AccountNumber))
                    return false;
                
                if (!IsValidRoutingNumber(paymentData.RoutingNumber))
                    return false;
                
                // Compliance checks
                var complianceService = new ComplianceService();
                var complianceResult = await complianceService.CheckTransferAsync(
                    paymentData.AccountNumber, order.Amount, order.CustomerId);
                
                if (!complianceResult.IsCompliant)
                    return false;
                
                // Different transfer process
                var transferRequest = new BankTransferRequest
                {
                    FromAccount = paymentData.AccountNumber,
                    ToAccount = "COMPANY-ACCOUNT-123",
                    Amount = order.Amount,
                    Reference = $"Order-{order.Id}",
                    ProcessingDate = DateTime.Now.AddDays(1) // Next business day
                };
                
                var transferResult = await bankingService.InitiateTransferAsync(transferRequest);
                
                if (transferResult.IsSuccessful)
                {
                    order.PaymentConfirmationCode = transferResult.TransferReference;
                    order.ExpectedPaymentDate = transferResult.ExpectedCompletionDate;
                    return true;
                }
            }
            
            throw new NotSupportedException($"Payment method '{paymentMethod}' is not supported");
        }
        catch (Exception ex)
        {
            // Generic error handling - doesn't know specifics of each payment method
            order.PaymentErrorMessage = ex.Message;
            return false;
        }
    }
    
    // Helper methods that clients must implement
    private bool IsValidEmail(string email)
    {
        // Email validation logic
        return email.Contains("@") && email.Contains(".");
    }
    
    private bool IsValidAccountNumber(string accountNumber)
    {
        // Account number validation
        return accountNumber.Length >= 8 && accountNumber.All(char.IsDigit);
    }
    
    private bool IsValidRoutingNumber(string routingNumber)
    {
        // Routing number validation
        return routingNumber.Length == 9 && routingNumber.All(char.IsDigit);
    }
}

// Supporting classes that expose too much complexity
public class PaymentData
{
    // Credit card fields
    public string CardNumber { get; set; }
    public string Cvv { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string CardHolderName { get; set; }
    
    // PayPal fields
    public string PayPalEmail { get; set; }
    
    // Bank transfer fields
    public string AccountNumber { get; set; }
    public string RoutingNumber { get; set; }
    public string BankName { get; set; }
    
    // Client must know which fields to use for which payment method!
}
```

### Problems with this approach

1. **High complexity** - Client must understand all payment method implementations
2. **Tight coupling** - OrderService is coupled to all payment APIs and services
3. **Hard to maintain** - Adding new payment methods requires modifying OrderService
4. **Code duplication** - Similar logic (validation, error handling) repeated for each method
5. **Testing difficulties** - Must mock all payment services and understand their details
6. **Violates Single Responsibility** - OrderService handles order logic AND payment processing details

## ✅ Correct Implementation (Proper Abstraction)

```csharp
// ✅ GOOD: Proper abstraction hides implementation complexity

// Abstract interface - defines what, not how
public interface IPaymentProcessor
{
    Task<PaymentResult> ProcessPaymentAsync(decimal amount, PaymentDetails paymentDetails);
    Task<bool> ValidatePaymentAsync(PaymentDetails paymentDetails);
    Task<PaymentResult> RefundPaymentAsync(string transactionId, decimal amount);
    PaymentMethodInfo GetPaymentMethodInfo();
    bool IsAvailable();
}

// Abstract payment details - hides method-specific details
public abstract class PaymentDetails
{
    public string CustomerEmail { get; set; }
    public Address BillingAddress { get; set; }
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

// Concrete payment detail classes
public class CreditCardPaymentDetails : PaymentDetails
{
    public string CardNumber { get; set; }
    public string CardHolderName { get; set; }
    public string Cvv { get; set; }
    public int ExpiryMonth { get; set; }
    public int ExpiryYear { get; set; }
}

public class PayPalPaymentDetails : PaymentDetails
{
    public string PayPalEmail { get; set; }
    public string ReturnUrl { get; set; }
    public string CancelUrl { get; set; }
}

public class BankTransferPaymentDetails : PaymentDetails
{
    public string AccountNumber { get; set; }
    public string RoutingNumber { get; set; }
    public string BankName { get; set; }
    public string AccountHolderName { get; set; }
}

// Payment result abstraction
public class PaymentResult
{
    public bool IsSuccessful { get; set; }
    public string TransactionId { get; set; }
    public string ConfirmationCode { get; set; }
    public string ErrorMessage { get; set; }
    public PaymentStatus Status { get; set; }
    public DateTime ProcessedAt { get; set; }
    public Dictionary<string, object> AdditionalInfo { get; set; } = new();
}

public class PaymentMethodInfo
{
    public string Name { get; set; }
    public string DisplayName { get; set; }
    public List<string> SupportedCurrencies { get; set; }
    public decimal MinimumAmount { get; set; }
    public decimal MaximumAmount { get; set; }
    public TimeSpan ProcessingTime { get; set; }
    public bool RequiresRedirect { get; set; }
}

public enum PaymentStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled,
    RequiresApproval,
    Refunded
}

// Concrete implementations - each handles its own complexity
public class CreditCardProcessor : IPaymentProcessor
{
    private readonly ICreditCardApiClient _apiClient;
    private readonly IEncryptionService _encryptionService;
    private readonly IFraudDetectionService _fraudDetection;
    private readonly ILogger<CreditCardProcessor> _logger;

    public CreditCardProcessor(
        ICreditCardApiClient apiClient,
        IEncryptionService encryptionService,
        IFraudDetectionService fraudDetection,
        ILogger<CreditCardProcessor> logger)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _fraudDetection = fraudDetection ?? throw new ArgumentNullException(nameof(fraudDetection));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, PaymentDetails paymentDetails)
    {
        if (!(paymentDetails is CreditCardPaymentDetails cardDetails))
            throw new ArgumentException("Invalid payment details type for credit card processor");

        try
        {
            _logger.LogInformation("Processing credit card payment for amount {Amount}", amount);

            // Internal validation - hidden from client
            if (!await ValidatePaymentAsync(paymentDetails))
            {
                return new PaymentResult
                {
                    IsSuccessful = false,
                    Status = PaymentStatus.Failed,
                    ErrorMessage = "Payment validation failed"
                };
            }

            // Internal fraud detection - hidden complexity
            var fraudRisk = await CheckFraudRiskAsync(cardDetails, amount);
            if (fraudRisk.IsHighRisk)
            {
                _logger.LogWarning("High fraud risk detected for card ending in {LastFour}", 
                    cardDetails.CardNumber.Substring(cardDetails.CardNumber.Length - 4));
                
                return new PaymentResult
                {
                    IsSuccessful = false,
                    Status = PaymentStatus.Failed,
                    ErrorMessage = "Transaction declined for security reasons"
                };
            }

            // Internal encryption - hidden from client
            var encryptedCard = _encryptionService.EncryptCardData(cardDetails);

            // Internal API communication - hidden complexity
            var apiRequest = BuildApiRequest(amount, encryptedCard, cardDetails);
            var apiResponse = await _apiClient.ProcessPaymentAsync(apiRequest);

            // Internal response processing - hidden from client
            return ProcessApiResponse(apiResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing credit card payment");
            return new PaymentResult
            {
                IsSuccessful = false,
                Status = PaymentStatus.Failed,
                ErrorMessage = "Payment processing error occurred"
            };
        }
    }

    public async Task<bool> ValidatePaymentAsync(PaymentDetails paymentDetails)
    {
        if (!(paymentDetails is CreditCardPaymentDetails cardDetails))
            return false;

        // All validation logic encapsulated here
        return ValidateCardNumber(cardDetails.CardNumber) &&
               ValidateExpiryDate(cardDetails.ExpiryMonth, cardDetails.ExpiryYear) &&
               ValidateCvv(cardDetails.Cvv) &&
               ValidateCardHolderName(cardDetails.CardHolderName);
    }

    public async Task<PaymentResult> RefundPaymentAsync(string transactionId, decimal amount)
    {
        try
        {
            _logger.LogInformation("Processing refund for transaction {TransactionId}, amount {Amount}", 
                transactionId, amount);

            var refundRequest = new RefundRequest
            {
                OriginalTransactionId = transactionId,
                RefundAmount = amount,
                Reason = "Customer refund request"
            };

            var response = await _apiClient.ProcessRefundAsync(refundRequest);
            
            return new PaymentResult
            {
                IsSuccessful = response.IsSuccessful,
                TransactionId = response.RefundTransactionId,
                Status = response.IsSuccessful ? PaymentStatus.Refunded : PaymentStatus.Failed,
                ErrorMessage = response.ErrorMessage,
                ProcessedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for transaction {TransactionId}", transactionId);
            return new PaymentResult
            {
                IsSuccessful = false,
                Status = PaymentStatus.Failed,
                ErrorMessage = "Refund processing error occurred"
            };
        }
    }

    public PaymentMethodInfo GetPaymentMethodInfo()
    {
        return new PaymentMethodInfo
        {
            Name = "CreditCard",
            DisplayName = "Credit Card",
            SupportedCurrencies = new List<string> { "USD", "EUR", "GBP", "CAD" },
            MinimumAmount = 1.00m,
            MaximumAmount = 10000.00m,
            ProcessingTime = TimeSpan.FromMinutes(1),
            RequiresRedirect = false
        };
    }

    public bool IsAvailable()
    {
        // Check if credit card processing is available
        return _apiClient.IsServiceAvailable();
    }

    // Private methods - internal implementation hidden from clients
    private async Task<FraudRiskResult> CheckFraudRiskAsync(CreditCardPaymentDetails cardDetails, decimal amount)
    {
        return await _fraudDetection.AssessRiskAsync(new FraudAssessmentRequest
        {
            CardNumber = cardDetails.CardNumber,
            Amount = amount,
            CustomerEmail = cardDetails.CustomerEmail,
            BillingAddress = cardDetails.BillingAddress
        });
    }

    private bool ValidateCardNumber(string cardNumber)
    {
        if (string.IsNullOrWhiteSpace(cardNumber))
            return false;

        // Remove spaces and dashes
        cardNumber = cardNumber.Replace(" ", "").Replace("-", "");

        // Check length and digits only
        if (cardNumber.Length < 13 || cardNumber.Length > 19 || !cardNumber.All(char.IsDigit))
            return false;

        // Luhn algorithm check
        return IsValidLuhnCheck(cardNumber);
    }

    private bool ValidateExpiryDate(int month, int year)
    {
        if (month < 1 || month > 12)
            return false;

        var expiryDate = new DateTime(year, month, 1).AddMonths(1).AddDays(-1);
        return expiryDate > DateTime.Now;
    }

    private bool ValidateCvv(string cvv)
    {
        return !string.IsNullOrWhiteSpace(cvv) && 
               cvv.Length >= 3 && 
               cvv.Length <= 4 && 
               cvv.All(char.IsDigit);
    }

    private bool ValidateCardHolderName(string name)
    {
        return !string.IsNullOrWhiteSpace(name) && 
               name.Length >= 2 && 
               name.Length <= 100;
    }

    private bool IsValidLuhnCheck(string cardNumber)
    {
        // Luhn algorithm implementation
        var sum = 0;
        var alternate = false;

        for (var i = cardNumber.Length - 1; i >= 0; i--)
        {
            var digit = int.Parse(cardNumber[i].ToString());

            if (alternate)
            {
                digit *= 2;
                if (digit > 9)
                    digit = (digit % 10) + 1;
            }

            sum += digit;
            alternate = !alternate;
        }

        return sum % 10 == 0;
    }

    private CreditCardApiRequest BuildApiRequest(decimal amount, EncryptedCardData encryptedCard, CreditCardPaymentDetails cardDetails)
    {
        return new CreditCardApiRequest
        {
            Amount = amount,
            Currency = "USD",
            EncryptedCardData = encryptedCard,
            BillingAddress = cardDetails.BillingAddress,
            TransactionId = Guid.NewGuid().ToString(),
            MerchantId = _apiClient.MerchantId
        };
    }

    private PaymentResult ProcessApiResponse(CreditCardApiResponse response)
    {
        return new PaymentResult
        {
            IsSuccessful = response.IsApproved,
            TransactionId = response.TransactionId,
            ConfirmationCode = response.AuthorizationCode,
            Status = MapApiStatusToPaymentStatus(response.StatusCode),
            ErrorMessage = response.IsApproved ? null : response.DeclineReason,
            ProcessedAt = response.ProcessedAt,
            AdditionalInfo = new Dictionary<string, object>
            {
                { "AuthorizationCode", response.AuthorizationCode },
                { "ProcessorResponse", response.ProcessorResponseCode }
            }
        };
    }

    private PaymentStatus MapApiStatusToPaymentStatus(string statusCode)
    {
        return statusCode switch
        {
            "00" => PaymentStatus.Completed,
            "05" => PaymentStatus.Failed,
            "51" => PaymentStatus.Failed,
            "91" => PaymentStatus.Processing,
            _ => PaymentStatus.Failed
        };
    }
}

public class PayPalProcessor : IPaymentProcessor
{
    private readonly IPayPalApiClient _paypalApi;
    private readonly ITokenService _tokenService;
    private readonly ILogger<PayPalProcessor> _logger;

    public PayPalProcessor(IPayPalApiClient paypalApi, ITokenService tokenService, ILogger<PayPalProcessor> logger)
    {
        _paypalApi = paypalApi ?? throw new ArgumentNullException(nameof(paypalApi));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<PaymentResult> ProcessPaymentAsync(decimal amount, PaymentDetails paymentDetails)
    {
        if (!(paymentDetails is PayPalPaymentDetails paypalDetails))
            throw new ArgumentException("Invalid payment details type for PayPal processor");

        try
        {
            _logger.LogInformation("Processing PayPal payment for amount {Amount}", amount);

            // Internal authentication - hidden from client
            var authToken = await AuthenticateAsync();

            // Internal payment creation - hidden complexity
            var paymentRequest = BuildPayPalPaymentRequest(amount, paypalDetails);
            var paymentResponse = await _paypalApi.CreatePaymentAsync(paymentRequest, authToken);

            if (paymentResponse.IsSuccessful)
            {
                return new PaymentResult
                {
                    IsSuccessful = true,
                    TransactionId = paymentResponse.PaymentId,
                    Status = PaymentStatus.RequiresApproval,
                    ProcessedAt = DateTime.UtcNow,
                    AdditionalInfo = new Dictionary<string, object>
                    {
                        { "ApprovalUrl", paymentResponse.ApprovalUrl },
                        { "PayPalPaymentId", paymentResponse.PaymentId }
                    }
                };
            }

            return new PaymentResult
            {
                IsSuccessful = false,
                Status = PaymentStatus.Failed,
                ErrorMessage = paymentResponse.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PayPal payment");
            return new PaymentResult
            {
                IsSuccessful = false,
                Status = PaymentStatus.Failed,
                ErrorMessage = "PayPal payment processing error occurred"
            };
        }
    }

    public async Task<bool> ValidatePaymentAsync(PaymentDetails paymentDetails)
    {
        if (!(paymentDetails is PayPalPaymentDetails paypalDetails))
            return false;

        return IsValidEmail(paypalDetails.PayPalEmail) &&
               !string.IsNullOrWhiteSpace(paypalDetails.ReturnUrl) &&
               !string.IsNullOrWhiteSpace(paypalDetails.CancelUrl);
    }

    public async Task<PaymentResult> RefundPaymentAsync(string transactionId, decimal amount)
    {
        try
        {
            var authToken = await AuthenticateAsync();
            var refundResponse = await _paypalApi.RefundPaymentAsync(transactionId, amount, authToken);

            return new PaymentResult
            {
                IsSuccessful = refundResponse.IsSuccessful,
                TransactionId = refundResponse.RefundId,
                Status = refundResponse.IsSuccessful ? PaymentStatus.Refunded : PaymentStatus.Failed,
                ErrorMessage = refundResponse.ErrorMessage,
                ProcessedAt = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing PayPal refund for transaction {TransactionId}", transactionId);
            return new PaymentResult
            {
                IsSuccessful = false,
                Status = PaymentStatus.Failed,
                ErrorMessage = "PayPal refund processing error occurred"
            };
        }
    }

    public PaymentMethodInfo GetPaymentMethodInfo()
    {
        return new PaymentMethodInfo
        {
            Name = "PayPal",
            DisplayName = "PayPal",
            SupportedCurrencies = new List<string> { "USD", "EUR", "GBP", "CAD", "AUD" },
            MinimumAmount = 0.01m,
            MaximumAmount = 60000.00m,
            ProcessingTime = TimeSpan.FromMinutes(5),
            RequiresRedirect = true
        };
    }

    public bool IsAvailable()
    {
        return _paypalApi.IsServiceAvailable();
    }

    // Private methods - internal implementation
    private async Task<AuthenticationToken> AuthenticateAsync()
    {
        return await _tokenService.GetPayPalAuthTokenAsync();
    }

    private bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }

    private PayPalPaymentRequest BuildPayPalPaymentRequest(decimal amount, PayPalPaymentDetails details)
    {
        return new PayPalPaymentRequest
        {
            Intent = "sale",
            Amount = amount,
            Currency = "USD",
            PayerEmail = details.PayPalEmail,
            ReturnUrl = details.ReturnUrl,
            CancelUrl = details.CancelUrl,
            Description = "Online purchase"
        };
    }
}

// Simplified OrderService using abstraction
public class OrderService
{
    private readonly IPaymentProcessorFactory _paymentProcessorFactory;
    private readonly IOrderRepository _orderRepository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IPaymentProcessorFactory paymentProcessorFactory,
        IOrderRepository orderRepository,
        ILogger<OrderService> logger)
    {
        _paymentProcessorFactory = paymentProcessorFactory ?? throw new ArgumentNullException(nameof(paymentProcessorFactory));
        _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Simple, clean method - abstraction hides all payment complexity
    public async Task<OrderResult> ProcessOrderAsync(Order order, string paymentMethod, PaymentDetails paymentDetails)
    {
        try
        {
            _logger.LogInformation("Processing order {OrderId} with payment method {PaymentMethod}", 
                order.Id, paymentMethod);

            // Get appropriate payment processor - factory pattern with abstraction
            var paymentProcessor = _paymentProcessorFactory.GetProcessor(paymentMethod);
            
            if (paymentProcessor == null)
            {
                return new OrderResult
                {
                    IsSuccessful = false,
                    ErrorMessage = $"Payment method '{paymentMethod}' is not supported"
                };
            }

            // Check if payment method is available
            if (!paymentProcessor.IsAvailable())
            {
                return new OrderResult
                {
                    IsSuccessful = false,
                    ErrorMessage = $"Payment method '{paymentMethod}' is temporarily unavailable"
                };
            }

            // Process payment - all complexity hidden behind abstraction
            var paymentResult = await paymentProcessor.ProcessPaymentAsync(order.Total, paymentDetails);

            if (paymentResult.IsSuccessful)
            {
                // Update order with payment information
                order.PaymentTransactionId = paymentResult.TransactionId;
                order.PaymentConfirmationCode = paymentResult.ConfirmationCode;
                order.PaymentStatus = paymentResult.Status;
                order.PaymentProcessedAt = paymentResult.ProcessedAt;

                // Save order
                await _orderRepository.UpdateAsync(order);

                return new OrderResult
                {
                    IsSuccessful = true,
                    OrderId = order.Id,
                    TransactionId = paymentResult.TransactionId,
                    ConfirmationCode = paymentResult.ConfirmationCode,
                    AdditionalInfo = paymentResult.AdditionalInfo
                };
            }
            else
            {
                return new OrderResult
                {
                    IsSuccessful = false,
                    OrderId = order.Id,
                    ErrorMessage = paymentResult.ErrorMessage
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing order {OrderId}", order.Id);
            return new OrderResult
            {
                IsSuccessful = false,
                OrderId = order.Id,
                ErrorMessage = "Order processing error occurred"
            };
        }
    }

    public async Task<RefundResult> RefundOrderAsync(int orderId, decimal refundAmount)
    {
        try
        {
            var order = await _orderRepository.GetByIdAsync(orderId);
            if (order == null)
                throw new ArgumentException($"Order {orderId} not found");

            // Determine payment processor from order
            var paymentProcessor = _paymentProcessorFactory.GetProcessorByTransactionId(order.PaymentTransactionId);
            
            // Process refund - complexity abstracted away
            var refundResult = await paymentProcessor.RefundPaymentAsync(order.PaymentTransactionId, refundAmount);

            // Update order status
            if (refundResult.IsSuccessful)
            {
                order.RefundTransactionId = refundResult.TransactionId;
                order.RefundAmount = refundAmount;
                order.RefundProcessedAt = refundResult.ProcessedAt;
                order.PaymentStatus = PaymentStatus.Refunded;

                await _orderRepository.UpdateAsync(order);
            }

            return new RefundResult
            {
                IsSuccessful = refundResult.IsSuccessful,
                RefundTransactionId = refundResult.TransactionId,
                ErrorMessage = refundResult.ErrorMessage
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing refund for order {OrderId}", orderId);
            return new RefundResult
            {
                IsSuccessful = false,
                ErrorMessage = "Refund processing error occurred"
            };
        }
    }
}

// Factory interface for creating payment processors
public interface IPaymentProcessorFactory
{
    IPaymentProcessor GetProcessor(string paymentMethod);
    IPaymentProcessor GetProcessorByTransactionId(string transactionId);
    IEnumerable<PaymentMethodInfo> GetAvailablePaymentMethods();
}

// Result classes
public class OrderResult
{
    public bool IsSuccessful { get; set; }
    public int OrderId { get; set; }
    public string TransactionId { get; set; }
    public string ConfirmationCode { get; set; }
    public string ErrorMessage { get; set; }
    public Dictionary<string, object> AdditionalInfo { get; set; } = new();
}

public class RefundResult
{
    public bool IsSuccessful { get; set; }
    public string RefundTransactionId { get; set; }
    public string ErrorMessage { get; set; }
}
```

## 🏢 ERP Example: Report Generation System

```csharp
// Abstract report generator - hides complexity of different report types
public abstract class ReportGenerator
{
    protected readonly IDataService _dataService;
    protected readonly ILogger _logger;

    protected ReportGenerator(IDataService dataService, ILogger logger)
    {
        _dataService = dataService ?? throw new ArgumentNullException(nameof(dataService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // Template method - defines the process, hides implementation details
    public async Task<ReportResult> GenerateReportAsync(ReportRequest request)
    {
        try
        {
            _logger.LogInformation("Starting report generation: {ReportType}", GetReportType());

            // Abstract steps - each subclass implements these differently
            ValidateRequest(request);
            var data = await CollectDataAsync(request);
            var processedData = ProcessData(data, request);
            var formattedReport = FormatReport(processedData, request);
            
            var result = new ReportResult
            {
                IsSuccessful = true,
                ReportType = GetReportType(),
                GeneratedAt = DateTime.UtcNow,
                Content = formattedReport,
                Format = GetOutputFormat()
            };

            await SaveReportAsync(result, request);
            
            _logger.LogInformation("Report generation completed: {ReportType}", GetReportType());
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating report: {ReportType}", GetReportType());
            return new ReportResult
            {
                IsSuccessful = false,
                ErrorMessage = ex.Message,
                ReportType = GetReportType()
            };
        }
    }

    // Abstract methods - subclasses provide specific implementations
    protected abstract string GetReportType();
    protected abstract string GetOutputFormat();
    protected abstract void ValidateRequest(ReportRequest request);
    protected abstract Task<ReportData> CollectDataAsync(ReportRequest request);
    protected abstract ReportData ProcessData(ReportData rawData, ReportRequest request);
    protected abstract byte[] FormatReport(ReportData processedData, ReportRequest request);
    
    // Common functionality - shared by all report types
    protected virtual async Task SaveReportAsync(ReportResult result, ReportRequest request)
    {
        if (request.SaveToStorage)
        {
            var fileName = $"{GetReportType()}_{DateTime.Now:yyyyMMdd_HHmmss}.{GetOutputFormat().ToLower()}";
            // Save logic here
            _logger.LogInformation("Report saved as {FileName}", fileName);
        }
    }

    protected virtual void LogReportGeneration(ReportRequest request)
    {
        _logger.LogInformation("Generating {ReportType} report for date range {StartDate} to {EndDate}", 
            GetReportType(), request.StartDate, request.EndDate);
    }
}

// Concrete implementations - each handles specific report complexity
public class FinancialReportGenerator : ReportGenerator
{
    private readonly IFinancialDataService _financialService;
    private readonly IExcelService _excelService;

    public FinancialReportGenerator(
        IDataService dataService, 
        IFinancialDataService financialService,
        IExcelService excelService,
        ILogger<FinancialReportGenerator> logger) 
        : base(dataService, logger)
    {
        _financialService = financialService;
        _excelService = excelService;
    }

    protected override string GetReportType() => "Financial";
    protected override string GetOutputFormat() => "XLSX";

    protected override void ValidateRequest(ReportRequest request)
    {
        if (request.StartDate >= request.EndDate)
            throw new ArgumentException("Start date must be before end date");

        if ((request.EndDate - request.StartDate).Days > 365)
            throw new ArgumentException("Date range cannot exceed one year");

        if (request.Filters?.ContainsKey("Department") == false)
            throw new ArgumentException("Department filter is required for financial reports");
    }

    protected override async Task<ReportData> CollectDataAsync(ReportRequest request)
    {
        var data = new ReportData();

        // Collect revenue data
        data.Revenue = await _financialService.GetRevenueDataAsync(
            request.StartDate, request.EndDate, request.Filters);

        // Collect expense data
        data.Expenses = await _financialService.GetExpenseDataAsync(
            request.StartDate, request.EndDate, request.Filters);

        // Collect budget data
        data.Budget = await _financialService.GetBudgetDataAsync(
            request.StartDate, request.EndDate, request.Filters);

        return data;
    }

    protected override ReportData ProcessData(ReportData rawData, ReportRequest request)
    {
        // Calculate financial metrics
        var processedData = new ReportData
        {
            Revenue = rawData.Revenue,
            Expenses = rawData.Expenses,
            Budget = rawData.Budget
        };

        // Calculate profit/loss
        processedData.ProfitLoss = CalculateProfitLoss(rawData.Revenue, rawData.Expenses);

        // Calculate budget variance
        processedData.BudgetVariance = CalculateBudgetVariance(rawData.Revenue, rawData.Expenses, rawData.Budget);

        // Calculate growth metrics
        processedData.GrowthMetrics = CalculateGrowthMetrics(rawData, request);

        return processedData;
    }

    protected override byte[] FormatReport(ReportData processedData, ReportRequest request)
    {
        // Use Excel service to create formatted financial report
        var workbook = _excelService.CreateWorkbook();
        
        // Summary sheet
        var summarySheet = _excelService.AddWorksheet(workbook, "Summary");
        AddFinancialSummary(summarySheet, processedData);

        // Revenue breakdown sheet
        var revenueSheet = _excelService.AddWorksheet(workbook, "Revenue");
        AddRevenueBreakdown(revenueSheet, processedData.Revenue);

        // Expense breakdown sheet
        var expenseSheet = _excelService.AddWorksheet(workbook, "Expenses");
        AddExpenseBreakdown(expenseSheet, processedData.Expenses);

        // Charts and visualizations
        AddFinancialCharts(workbook, processedData);

        return _excelService.SaveWorkbookToBytes(workbook);
    }

    // Private methods handle specific financial report complexity
    private Dictionary<string, decimal> CalculateProfitLoss(
        Dictionary<string, decimal> revenue, 
        Dictionary<string, decimal> expenses)
    {
        var result = new Dictionary<string, decimal>();
        
        foreach (var revenueItem in revenue)
        {
            var correspondingExpense = expenses.GetValueOrDefault(revenueItem.Key, 0);
            result[revenueItem.Key] = revenueItem.Value - correspondingExpense;
        }

        return result;
    }

    private Dictionary<string, decimal> CalculateBudgetVariance(
        Dictionary<string, decimal> revenue,
        Dictionary<string, decimal> expenses,
        Dictionary<string, decimal> budget)
    {
        // Budget variance calculation logic
        var result = new Dictionary<string, decimal>();
        // Implementation details...
        return result;
    }

    private GrowthMetrics CalculateGrowthMetrics(ReportData data, ReportRequest request)
    {
        // Growth calculation logic
        return new GrowthMetrics();
    }

    private void AddFinancialSummary(IWorksheet sheet, ReportData data)
    {
        // Excel formatting for financial summary
    }

    private void AddRevenueBreakdown(IWorksheet sheet, Dictionary<string, decimal> revenue)
    {
        // Revenue breakdown formatting
    }

    private void AddExpenseBreakdown(IWorksheet sheet, Dictionary<string, decimal> expenses)
    {
        // Expense breakdown formatting
    }

    private void AddFinancialCharts(IWorkbook workbook, ReportData data)
    {
        // Chart creation logic
    }
}

public class InventoryReportGenerator : ReportGenerator
{
    private readonly IInventoryDataService _inventoryService;
    private readonly IPdfService _pdfService;

    public InventoryReportGenerator(
        IDataService dataService,
        IInventoryDataService inventoryService,
        IPdfService pdfService,
        ILogger<InventoryReportGenerator> logger)
        : base(dataService, logger)
    {
        _inventoryService = inventoryService;
        _pdfService = pdfService;
    }

    protected override string GetReportType() => "Inventory";
    protected override string GetOutputFormat() => "PDF";

    protected override void ValidateRequest(ReportRequest request)
    {
        if (request.Filters?.ContainsKey("Location") == false)
            throw new ArgumentException("Location filter is required for inventory reports");
    }

    protected override async Task<ReportData> CollectDataAsync(ReportRequest request)
    {
        var data = new ReportData();

        // Collect inventory levels
        data.InventoryLevels = await _inventoryService.GetInventoryLevelsAsync(
            request.StartDate, request.EndDate, request.Filters);

        // Collect movement data
        data.InventoryMovements = await _inventoryService.GetMovementDataAsync(
            request.StartDate, request.EndDate, request.Filters);

        // Collect reorder points
        data.ReorderData = await _inventoryService.GetReorderPointsAsync(request.Filters);

        return data;
    }

    protected override ReportData ProcessData(ReportData rawData, ReportRequest request)
    {
        var processedData = new ReportData
        {
            InventoryLevels = rawData.InventoryLevels,
            InventoryMovements = rawData.InventoryMovements,
            ReorderData = rawData.ReorderData
        };

        // Calculate inventory metrics
        processedData.LowStockItems = IdentifyLowStockItems(rawData);
        processedData.ExcessStockItems = IdentifyExcessStockItems(rawData);
        processedData.TurnoverRates = CalculateTurnoverRates(rawData);

        return processedData;
    }

    protected override byte[] FormatReport(ReportData processedData, ReportRequest request)
    {
        // Create PDF inventory report
        var document = _pdfService.CreateDocument();

        // Add title page
        AddInventoryReportTitle(document, request);

        // Add executive summary
        AddInventorySummary(document, processedData);

        // Add detailed inventory tables
        AddInventoryTables(document, processedData);

        // Add low stock alerts
        AddLowStockAlerts(document, processedData.LowStockItems);

        // Add charts and graphs
        AddInventoryCharts(document, processedData);

        return _pdfService.SaveDocumentToBytes(document);
    }

    // Private methods for inventory-specific processing
    private List<InventoryItem> IdentifyLowStockItems(ReportData data)
    {
        // Low stock identification logic
        return new List<InventoryItem>();
    }

    private List<InventoryItem> IdentifyExcessStockItems(ReportData data)
    {
        // Excess stock identification logic
        return new List<InventoryItem>();
    }

    private Dictionary<string, decimal> CalculateTurnoverRates(ReportData data)
    {
        // Turnover rate calculation
        return new Dictionary<string, decimal>();
    }

    private void AddInventoryReportTitle(IPdfDocument document, ReportRequest request)
    {
        // PDF title formatting
    }

    private void AddInventorySummary(IPdfDocument document, ReportData data)
    {
        // Summary section formatting
    }

    private void AddInventoryTables(IPdfDocument document, ReportData data)
    {
        // Table formatting
    }

    private void AddLowStockAlerts(IPdfDocument document, List<InventoryItem> lowStockItems)
    {
        // Alert section formatting
    }

    private void AddInventoryCharts(IPdfDocument document, ReportData data)
    {
        // Chart creation and formatting
    }
}

// Simple client usage - all complexity abstracted away
public class ReportingService
{
    private readonly Dictionary<string, ReportGenerator> _reportGenerators;
    private readonly ILogger<ReportingService> _logger;

    public ReportingService(IEnumerable<ReportGenerator> generators, ILogger<ReportingService> logger)
    {
        _reportGenerators = generators.ToDictionary(g => g.GetType().Name.Replace("Generator", ""), g => g);
        _logger = logger;
    }

    // Simple method - abstraction hides all report generation complexity
    public async Task<ReportResult> GenerateReportAsync(string reportType, ReportRequest request)
    {
        _logger.LogInformation("Generating {ReportType} report", reportType);

        if (!_reportGenerators.TryGetValue($"{reportType}Report", out var generator))
        {
            return new ReportResult
            {
                IsSuccessful = false,
                ErrorMessage = $"Report type '{reportType}' is not supported"
            };
        }

        // All complexity handled by appropriate generator
        return await generator.GenerateReportAsync(request);
    }

    public IEnumerable<string> GetAvailableReportTypes()
    {
        return _reportGenerators.Keys.Select(k => k.Replace("Report", ""));
    }
}

// Supporting classes
public class ReportRequest
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Dictionary<string, object> Filters { get; set; } = new();
    public bool SaveToStorage { get; set; }
    public string OutputFormat { get; set; }
}

public class ReportResult
{
    public bool IsSuccessful { get; set; }
    public string ReportType { get; set; }
    public DateTime GeneratedAt { get; set; }
    public byte[] Content { get; set; }
    public string Format { get; set; }
    public string ErrorMessage { get; set; }
}

public class ReportData
{
    public Dictionary<string, decimal> Revenue { get; set; } = new();
    public Dictionary<string, decimal> Expenses { get; set; } = new();
    public Dictionary<string, decimal> Budget { get; set; } = new();
    public Dictionary<string, decimal> ProfitLoss { get; set; } = new();
    public Dictionary<string, decimal> BudgetVariance { get; set; } = new();
    public GrowthMetrics GrowthMetrics { get; set; }
    
    public Dictionary<string, int> InventoryLevels { get; set; } = new();
    public List<InventoryMovement> InventoryMovements { get; set; } = new();
    public Dictionary<string, int> ReorderData { get; set; } = new();
    public List<InventoryItem> LowStockItems { get; set; } = new();
    public List<InventoryItem> ExcessStockItems { get; set; } = new();
    public Dictionary<string, decimal> TurnoverRates { get; set; } = new();
}

public class GrowthMetrics
{
    public decimal RevenueGrowth { get; set; }
    public decimal ExpenseGrowth { get; set; }
    public decimal ProfitGrowth { get; set; }
}

public class InventoryItem
{
    public string ItemCode { get; set; }
    public string Description { get; set; }
    public int CurrentStock { get; set; }
    public int MinimumStock { get; set; }
    public int MaximumStock { get; set; }
    public decimal UnitCost { get; set; }
}

public class InventoryMovement
{
    public DateTime Date { get; set; }
    public string ItemCode { get; set; }
    public string MovementType { get; set; }
    public int Quantity { get; set; }
    public string Reference { get; set; }
}
```

## 🧪 Unit Testing Abstraction

```csharp
[TestFixture]
public class PaymentProcessorAbstractionTests
{
    private Mock<ICreditCardApiClient> _mockApiClient;
    private Mock<IEncryptionService> _mockEncryption;
    private Mock<IFraudDetectionService> _mockFraudDetection;
    private Mock<ILogger<CreditCardProcessor>> _mockLogger;
    private CreditCardProcessor _processor;

    [SetUp]
    public void Setup()
    {
        _mockApiClient = new Mock<ICreditCardApiClient>();
        _mockEncryption = new Mock<IEncryptionService>();
        _mockFraudDetection = new Mock<IFraudDetectionService>();
        _mockLogger = new Mock<ILogger<CreditCardProcessor>>();

        _processor = new CreditCardProcessor(
            _mockApiClient.Object,
            _mockEncryption.Object,
            _mockFraudDetection.Object,
            _mockLogger.Object);
    }

    [Test]
    public async Task ProcessPaymentAsync_Should_Return_Success_For_Valid_Payment()
    {
        // Arrange
        var paymentDetails = new CreditCardPaymentDetails
        {
            CardNumber = "4111111111111111", // Valid test card
            CardHolderName = "John Doe",
            Cvv = "123",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            CustomerEmail = "john@example.com"
        };

        _mockFraudDetection.Setup(f => f.AssessRiskAsync(It.IsAny<FraudAssessmentRequest>()))
            .ReturnsAsync(new FraudRiskResult { IsHighRisk = false });

        _mockEncryption.Setup(e => e.EncryptCardData(It.IsAny<CreditCardPaymentDetails>()))
            .Returns(new EncryptedCardData());

        _mockApiClient.Setup(a => a.ProcessPaymentAsync(It.IsAny<CreditCardApiRequest>()))
            .ReturnsAsync(new CreditCardApiResponse
            {
                IsApproved = true,
                TransactionId = "TXN123",
                AuthorizationCode = "AUTH456",
                StatusCode = "00",
                ProcessedAt = DateTime.UtcNow
            });

        // Act
        var result = await _processor.ProcessPaymentAsync(100.00m, paymentDetails);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("TXN123", result.TransactionId);
        Assert.AreEqual("AUTH456", result.ConfirmationCode);
        Assert.AreEqual(PaymentStatus.Completed, result.Status);
    }

    [Test]
    public async Task ProcessPaymentAsync_Should_Reject_High_Fraud_Risk()
    {
        // Arrange
        var paymentDetails = CreateValidCreditCardDetails();

        _mockFraudDetection.Setup(f => f.AssessRiskAsync(It.IsAny<FraudAssessmentRequest>()))
            .ReturnsAsync(new FraudRiskResult { IsHighRisk = true });

        // Act
        var result = await _processor.ProcessPaymentAsync(100.00m, paymentDetails);

        // Assert
        Assert.IsFalse(result.IsSuccessful);
        Assert.AreEqual(PaymentStatus.Failed, result.Status);
        Assert.That(result.ErrorMessage, Contains.Substring("security reasons"));

        // Verify that API was not called due to fraud detection
        _mockApiClient.Verify(a => a.ProcessPaymentAsync(It.IsAny<CreditCardApiRequest>()), Times.Never);
    }

    [Test]
    public void ValidatePaymentAsync_Should_Reject_Invalid_Card_Numbers()
    {
        // Arrange
        var invalidCardDetails = new CreditCardPaymentDetails
        {
            CardNumber = "1234", // Invalid card number
            CardHolderName = "John Doe",
            Cvv = "123",
            ExpiryMonth = 12,
            ExpiryYear = 2025
        };

        // Act & Assert
        Assert.IsFalse(_processor.ValidatePaymentAsync(invalidCardDetails).Result);
    }

    [Test]
    public void GetPaymentMethodInfo_Should_Return_Correct_Information()
    {
        // Act
        var info = _processor.GetPaymentMethodInfo();

        // Assert
        Assert.AreEqual("CreditCard", info.Name);
        Assert.AreEqual("Credit Card", info.DisplayName);
        Assert.Contains("USD", info.SupportedCurrencies);
        Assert.AreEqual(1.00m, info.MinimumAmount);
        Assert.AreEqual(10000.00m, info.MaximumAmount);
        Assert.IsFalse(info.RequiresRedirect);
    }

    private CreditCardPaymentDetails CreateValidCreditCardDetails()
    {
        return new CreditCardPaymentDetails
        {
            CardNumber = "4111111111111111",
            CardHolderName = "John Doe",
            Cvv = "123",
            ExpiryMonth = 12,
            ExpiryYear = 2025,
            CustomerEmail = "john@example.com"
        };
    }
}

[TestFixture]
public class OrderServiceAbstractionTests
{
    private Mock<IPaymentProcessorFactory> _mockFactory;
    private Mock<IPaymentProcessor> _mockProcessor;
    private Mock<IOrderRepository> _mockRepository;
    private Mock<ILogger<OrderService>> _mockLogger;
    private OrderService _orderService;

    [SetUp]
    public void Setup()
    {
        _mockFactory = new Mock<IPaymentProcessorFactory>();
        _mockProcessor = new Mock<IPaymentProcessor>();
        _mockRepository = new Mock<IOrderRepository>();
        _mockLogger = new Mock<ILogger<OrderService>>();

        _orderService = new OrderService(_mockFactory.Object, _mockRepository.Object, _mockLogger.Object);
    }

    [Test]
    public async Task ProcessOrderAsync_Should_Handle_Any_Payment_Method_Through_Abstraction()
    {
        // Arrange
        var order = new Order { Id = 1, Total = 100.00m };
        var paymentDetails = new CreditCardPaymentDetails();

        _mockFactory.Setup(f => f.GetProcessor("creditcard"))
            .Returns(_mockProcessor.Object);

        _mockProcessor.Setup(p => p.IsAvailable())
            .Returns(true);

        _mockProcessor.Setup(p => p.ProcessPaymentAsync(100.00m, paymentDetails))
            .ReturnsAsync(new PaymentResult
            {
                IsSuccessful = true,
                TransactionId = "TXN123",
                ConfirmationCode = "CONF456",
                Status = PaymentStatus.Completed,
                ProcessedAt = DateTime.UtcNow
            });

        // Act
        var result = await _orderService.ProcessOrderAsync(order, "creditcard", paymentDetails);

        // Assert
        Assert.IsTrue(result.IsSuccessful);
        Assert.AreEqual("TXN123", result.TransactionId);
        Assert.AreEqual("CONF456", result.ConfirmationCode);

        // Verify abstraction was used correctly
        _mockFactory.Verify(f => f.GetProcessor("creditcard"), Times.Once);
        _mockProcessor.Verify(p => p.ProcessPaymentAsync(100.00m, paymentDetails), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(order), Times.Once);
    }

    [Test]
    public async Task ProcessOrderAsync_Should_Handle_Unsupported_Payment_Method()
    {
        // Arrange
        var order = new Order { Id = 1, Total = 100.00m };
        var paymentDetails = new CreditCardPaymentDetails();

        _mockFactory.Setup(f => f.GetProcessor("unsupported"))
            .Returns((IPaymentProcessor)null);

        // Act
        var result = await _orderService.ProcessOrderAsync(order, "unsupported", paymentDetails);

        // Assert
        Assert.IsFalse(result.IsSuccessful);
        Assert.That(result.ErrorMessage, Contains.Substring("not supported"));
    }
}
```

## ✅ Benefits of Proper Abstraction

1. **Simplified interfaces** - Clients work with essential operations only
2. **Implementation flexibility** - Can change internal implementation without affecting clients
3. **Reduced coupling** - Clients depend on abstractions, not concrete implementations
4. **Enhanced maintainability** - Changes to implementation details don't propagate to clients
5. **Better testability** - Easy to mock abstract interfaces for testing
6. **Code reusability** - Same abstract interface can work with multiple implementations

## 🎯 When to Use Abstraction

- **Complex operations** - When implementation involves many steps or external dependencies
- **Multiple implementations** - When different implementations of the same concept exist
- **API design** - When creating libraries or services for other developers
- **Plugin systems** - When you want to allow extensibility without code changes
- **Testing** - When you need to isolate components for unit testing

## 🚨 Common Mistakes

1. **Over-abstraction** - Creating abstractions for simple operations that don't need them
2. **Leaky abstractions** - Exposing implementation details through the abstract interface
3. **Wrong level of abstraction** - Too high-level (not useful) or too low-level (not abstract enough)
4. **Missing abstractions** - Not abstracting complex operations that would benefit from it
5. **Static dependencies** - Using concrete classes instead of abstract interfaces

## 🎯 Interview Questions

**Q: What's the difference between abstraction and encapsulation?**
**A:** Abstraction focuses on hiding implementation complexity and showing only essential features, while encapsulation bundles data and methods together and controls access. Abstraction is about interface design, encapsulation is about data protection.

**Q: How does abstraction help with maintainability?**
**A:** Abstraction creates stable interfaces that hide implementation details. This means internal changes don't affect client code, making the system easier to maintain and evolve without breaking existing functionality.

**Q: Give an example where abstraction significantly improves an ERP system.**
**A:** In an ERP system, abstracting payment processing allows the same order processing code to work with credit cards, PayPal, bank transfers, and future payment methods without modification, while hiding the complexity of each payment provider's API.

**Q: When might abstraction hurt performance?**
**A:** Abstraction can add overhead through virtual method calls, interface lookups, and additional layers. However, this is usually negligible compared to the benefits, and performance-critical code can be optimized while maintaining abstraction at the architectural level.

## 📝 Checklist

- [ ] Complex operations are hidden behind simple interfaces
- [ ] Clients only know about essential operations, not implementation details
- [ ] Abstract interfaces define contracts, not implementations
- [ ] Multiple implementations can be swapped without affecting clients
- [ ] Implementation details can change without breaking client code
- [ ] Abstractions are at the right level - neither too high nor too low
- [ ] Testing is simplified through mockable abstractions

---

**Previous**: [← Encapsulation](./01-encapsulation.md) | **Next**: [Inheritance →](./03-inheritance.md)