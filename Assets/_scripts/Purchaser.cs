using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;
using UnityEngine.Purchasing.Default;

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, 
// one of the existing Survival Shooter scripts.
namespace CompleteProject
{



    // Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
    public class Purchaser : MonoBehaviour, IStoreListener
    {
        private static IStoreController m_StoreController;          // The Unity Purchasing system.
        private static IExtensionProvider m_StoreExtensionProvider; // The store-specific Purchasing subsystems.
        private IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;
        private IAppleExtensions m_AppleExtensions;
        // Product identifiers for all products capable of being purchased: 
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values 
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        //public static string kProductIDConsumable = "consumable";
        public static string kProductIDConsumable = "booster_1";
        public static string kProductIDNonConsumable = "nonconsumable";
        public static string kProductIDSubscription = "subscription";

        // Apple App Store-specific product identifier for the subscription product.
        private static string kProductNameAppleSubscription = "com.unity3d.subscription.new";

        // Google Play Store-specific product identifier subscription product.
        private static string kProductNameGooglePlaySubscription = "gof.feedthespider";
        //private static string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

        public void Start()
        {
            //Debug.Log("m_StoreController: " + m_StoreController);
            // If we haven't set up the Unity Purchasing reference
            if (m_StoreController == null)
            {
                // Begin to configure our connection to Purchasing
                InitializePurchasing();
            }
        }

        public void InitializePurchasing()
        {
            Debug.Log("InitializePurchasing");
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			// Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.
/*
            builder.AddProduct("com.mysterytag.spider.booster_green_1", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.booster_blue_1", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.booster_purple_1", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.booster_green_10", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.booster_blue_10", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.booster_purple_10", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_1_free", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_2_free", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_3_free", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_4_free", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_1_green_payers", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_1_blue_payers", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_1_purple_payers", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_2_green_payers", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_2_blue_payers", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_2_purple_payers", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_3_green_payers", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_3_blue_payers", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.sale_3_purple_payers", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.energy_for_day", ProductType.Consumable);
            builder.AddProduct("com.mysterytag.spider.chapter", ProductType.Consumable);
*/
            builder.AddProduct("com.evogames.feedthespider.booster_green_1", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.booster_blue_1", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.booster_purple_1", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.booster_green_10", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.booster_blue_10", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.booster_purple_10", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_1_free", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_2_free", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_3_free", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_4_free", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_1_green_payers", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_1_blue_payers", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_1_purple_payers", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_2_green_payers", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_2_blue_payers", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_2_purple_payers", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_3_green_payers", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_3_blue_payers", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.sale_3_purple_payers", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.energy_for_day", ProductType.Consumable);
            builder.AddProduct("com.evogames.feedthespider.chapter", ProductType.Consumable);


            // Continue adding the non-consumable product.
            //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
            // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            // if the Product ID was configured differently between Apple and Google stores. Also note that
            // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
            // must only be referenced here. 
            /*
            builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
                { kProductNameAppleSubscription, AppleAppStore.Name },
                { kProductNameGooglePlaySubscription, GooglePlay.Name },
            });
            */
            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);


        }


        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return m_StoreController != null && m_StoreExtensionProvider != null;
        }


        public void BuyConsumable()
        {
            // Buy the consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDConsumable);
        }


        public void BuyNonConsumable()
        {
            // Buy the non-consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDNonConsumable);
        }


        public void BuySubscription()
        {
            // Buy the subscription product using its the general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            // Notice how we use the general product identifier in spite of this ID being mapped to
            // custom store-specific identifiers above.
            BuyProductID(kProductIDSubscription);
        }


        public void BuyProductID(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = m_StoreController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    m_StoreController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = m_StoreExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
                
				apple.RestoreTransactions((result) => {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }


        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("market OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            m_StoreController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            m_StoreExtensionProvider = extensions;

            m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
            m_AppleExtensions = extensions.GetExtension<IAppleExtensions>();

            int vip = 0;

            Dictionary<string, string> introductory_info_dict = m_AppleExtensions.GetIntroductoryPriceDictionary();

            //controller.products.all.First().metadata.localizedPrice
            foreach (var item in controller.products.all)
            {
                //Debug.Log(product.metadata.localizedPrice);
                //Debug.Log(product.metadata.isoCurrencyCode);
                //Debug.Log(product.metadata.localizedDescription);
                //Debug.Log(product.metadata.localizedPriceString);
                //Debug.Log(product.metadata.localizedTitle);

               // Fetch the currency Product reference from Unity Purchasing
                //Debug.Log(product.definition.id);
//#if UNITY_ANDROID
                if (item.definition.id.Length > 28)
                {
                    if (staticClass.prices.ContainsKey(item.definition.id.Substring(27))) staticClass.prices[item.definition.id.Substring(27)] = item.metadata.localizedPriceString;
                }
                //#elif UNITY_IOS
                //                if (product.definition.id.Length > 23)
                //                {
                //                    if (staticClass.prices.ContainsKey(product.definition.id.Substring(22))) staticClass.prices[product.definition.id.Substring(22)] = product.metadata.localizedPriceString;
                //                }
                //#endif                         if (item.availableToPurchase)
                {
                    Debug.Log(string.Join(" - ",
                        new[]
                        {
                        item.metadata.localizedTitle,
                        item.metadata.localizedDescription,
                        item.metadata.isoCurrencyCode,
                        item.metadata.localizedPrice.ToString(),
                        item.metadata.localizedPriceString,
                        item.transactionID,
                        item.receipt
                        }));

                    // this is the usage of SubscriptionManager class
                    if (item.receipt != null) {
                        if (item.definition.type == ProductType.Subscription) {
                            if (checkIfProductIsAvailableForSubscriptionManager(item.receipt)) {
                                string intro_json = (introductory_info_dict == null || !introductory_info_dict.ContainsKey(item.definition.storeSpecificId)) ? null : introductory_info_dict[item.definition.storeSpecificId];
                                SubscriptionManager p = new SubscriptionManager(item, intro_json);
                                SubscriptionInfo info = p.getSubscriptionInfo();
                                Debug.Log("product id is: " + info.getProductId());
                                Debug.Log("purchase date is: " + info.getPurchaseDate());
                                Debug.Log("subscription next billing date is: " + info.getExpireDate());
                                Debug.Log("is subscribed? " + info.isSubscribed().ToString());

                                if (info.isSubscribed().ToString() == "True") vip = 1;

                                Debug.Log("is expired? " + info.isExpired().ToString());
                                Debug.Log("is cancelled? " + info.isCancelled());
                                Debug.Log("product is in free trial peroid? " + info.isFreeTrial());
                                Debug.Log("product is auto renewing? " + info.isAutoRenewing());
                                Debug.Log("subscription remaining valid time until next billing date is: " + info.getRemainingTime());
                                Debug.Log("is this product in introductory price period? " + info.isIntroductoryPricePeriod());
                                Debug.Log("the product introductory localized price is: " + info.getIntroductoryPrice());
                                Debug.Log("the product introductory price period is: " + info.getIntroductoryPricePeriod());
                                Debug.Log("the number of product introductory price period cycles is: " + info.getIntroductoryPricePeriodCycles());
                            }
                            else {
                                Debug.Log("This product is not available for SubscriptionManager class, only products that are purchase by 1.19+ SDK can use this class.");
                            }
                        }
                        else {
                            Debug.Log("the product is not a subscription product");
                        }
                    }
                    else {
                        Debug.Log("the product should have a valid receipt");
                    }

                }



            }



            //extensions.GetExtension<googl>()
            if (vip == 0) {
                ctrProgressClass.progress["vip"] = 0;
                ctrProgressClass.saveProgress();
            }
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            Debug.Log("ProcessPurchase");
            bool validPurchase = true; // Presume valid for platforms with no R.V.
            string transactionId = "";
            // Unity IAP's validation logic is only included on these platforms.
            //#if UNITY_ANDROID || UNITY_STANDALONE_OSX
            
#if UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX
            // Prepare the validator with the secrets we prepared in the Editor
            // obfuscation window.

            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

            try
            {
                Debug.Log(args.purchasedProduct.metadata.isoCurrencyCode);
                Debug.Log(args.purchasedProduct.metadata.localizedPrice);
                Debug.Log(args.purchasedProduct.metadata.localizedPriceString);
               
                 var result = validator.Validate(args.purchasedProduct.receipt);
                Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    Debug.Log(productReceipt.productID);
                    Debug.Log(productReceipt.purchaseDate);
                    Debug.Log(productReceipt.transactionID);
                    transactionId = productReceipt.transactionID;
                    GooglePlayReceipt google = productReceipt as GooglePlayReceipt;
                    if (null != google)
                    {
                        // This is Google's Order ID.
                        // Note that it is null when testing in the sandbox
                        // because Google's sandbox does not provide Order IDs.
                        Debug.Log(google.transactionID);
                        Debug.Log(google.purchaseState);
                        Debug.Log(google.purchaseToken);
                        if (transactionId == "" || transactionId == null) transactionId = google.transactionID;
                        if (transactionId == "" || transactionId == null) transactionId = google.purchaseToken;
                    }

                    AppleInAppPurchaseReceipt apple = productReceipt as AppleInAppPurchaseReceipt;
                    if (null != apple)
                    {
                        Debug.Log(apple.originalTransactionIdentifier);
                        Debug.Log(apple.subscriptionExpirationDate);
                        Debug.Log(apple.cancellationDate);
                        Debug.Log(apple.quantity);
                        transactionId = apple.originalTransactionIdentifier;
                    }
                    marketClass.instance.setRewardForPurchase(productReceipt.productID, transactionId, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice);
                }
            }
            catch (IAPSecurityException)
            {
                Debug.Log("Invalid receipt, not unlocking content");
                validPurchase = false;
                //for test on desktop
                //fix
				//marketClass.instance.setRewardForPurchase(args.purchasedProduct.definition.id, transactionId, args.purchasedProduct.metadata.isoCurrencyCode, args.purchasedProduct.metadata.localizedPrice);

            }
#endif

            if (validPurchase)
            {
                // Unlock the appropriate content here.


            }














            // A consumable product has been purchased by this user.
            /*
            if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                //ScoreManager.score += 100;
            }
            // Or ... a non-consumable product has been purchased by this user.
            else if (String.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
            }
            // Or ... a subscription product has been purchased by this user.
            else if (String.Equals(args.purchasedProduct.definition.id, kProductIDSubscription, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // TODO: The subscription item has been successfully purchased, grant this to the player.
            }
            // Or ... an unknown product has been purchased by this user. Fill in additional products here....
            else
            {
                
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }
            */
            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Complete;
        }



        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }

        private bool checkIfProductIsAvailableForSubscriptionManager(string receipt) {
            var receipt_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(receipt);
            if (!receipt_wrapper.ContainsKey("Store") || !receipt_wrapper.ContainsKey("Payload")) {
                Debug.Log("The product receipt does not contain enough information");
                return false;
            }
            var store = (string)receipt_wrapper["Store"];
            var payload = (string)receipt_wrapper["Payload"];

            if (payload != null) {
                switch (store) {
                    case GooglePlay.Name: {
                            var payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(payload);
                            if (!payload_wrapper.ContainsKey("json")) {
                                Debug.Log("The product receipt does not contain enough information, the 'json' field is missing");
                                return false;
                            }
                            var original_json_payload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode((string)payload_wrapper["json"]);
                            if (original_json_payload_wrapper == null || !original_json_payload_wrapper.ContainsKey("developerPayload")) {
                                Debug.Log("The product receipt does not contain enough information, the 'developerPayload' field is missing");
                                return false;
                            }
                            var developerPayloadJSON = (string)original_json_payload_wrapper["developerPayload"];
                            var developerPayload_wrapper = (Dictionary<string, object>)MiniJson.JsonDecode(developerPayloadJSON);
                            if (developerPayload_wrapper == null || !developerPayload_wrapper.ContainsKey("is_free_trial") || !developerPayload_wrapper.ContainsKey("has_introductory_price_trial")) {
                                Debug.Log("The product receipt does not contain enough information, the product is not purchased using 1.19 or later");
                                return false;
                            }
                            return true;
                        }
                    case AppleAppStore.Name:
                    case AmazonApps.Name:
                    case MacAppStore.Name: {
                            return true;
                        }
                    default: {
                            return false;
                        }
                }
            }
            return false;
        }


    }
}