using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;

// Placing the Purchaser class in the CompleteProject namespace allows it to interact with ScoreManager, one of the existing Survival Shooter scripts.
namespace CompleteProject
{

	// Deriving the Purchaser class from IStoreListener enables it to receive messages from Unity Purchasing.
	public class Purchaser : MonoBehaviour, IStoreListener
	{
		private static IStoreController m_StoreController;                                                                  // Reference to the Purchasing system.
		private static IExtensionProvider m_StoreExtensionProvider;                                                         // Reference to store-specific Purchasing subsystems.

		// The purchase callback
		private Action<CoinPackage> _purchaseCallback;

		// Product identifiers for all products capable of being purchased: "convenience" general identifiers for use with Purchasing, and their store-specific identifier counterparts 
		// for use with and outside of Unity Purchasing. Define store-specific identifiers also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

		private static string kProductIDConsumable1 =    "consumable1";  // General handle for the consumable product.
		private static string kProductIDConsumable2 =    "consumable2";  // General handle for the consumable product.
		private static string kProductIDConsumable3 =    "consumable3";  // General handle for the consumable product.
		private static string kProductIDConsumable4 =    "consumable4";  // General handle for the consumable product.
		private static string kProductIDNonConsumable = "nonconsumable"; // General handle for the non-consumable product.
		private static string kProductIDSubscription =  "subscription";  // General handle for the subscription product.

		private static string kProductNameGooglePlayConsumable1 =    "pack1";        // Google Play Store identifier for the consumable product.
		private static string kProductNameGooglePlayConsumable2 =    "pack2";        // Google Play Store identifier for the consumable product.
		private static string kProductNameGooglePlayConsumable3 =    "pack3";        // Google Play Store identifier for the consumable product.
		private static string kProductNameGooglePlayConsumable4 =    "pack4";        // Google Play Store identifier for the consumable product.
		private static string kProductNameGooglePlayNonConsumable = "com.puzzlegame.thepuzzlefree";      // Google Play Store identifier for the non-consumable product.
		private static string kProductNameGooglePlaySubscription =  "NA";  // Google Play Store identifier for the subscription product.

		private string kProductNameAppleConsumable1 =    "com.xbean.Game01.500coins";      // Apple App Store identifier for the consumable product.
		private string kProductNameAppleConsumable2 =    "com.xbean.Game01.3000coins";      // Apple App Store identifier for the consumable product.
		private string kProductNameAppleConsumable3 =    "com.xbean.Game01.6500coins";      // Apple App Store identifier for the consumable product.
		private string kProductNameAppleConsumable4 =    "com.xbean.Game01.14000coins";      // Apple App Store identifier for the consumable product.
		private string kProductNameAppleNonConsumable = "com.puzzlegame.thepuzzlefree";      // Apple App Store identifier for the non-consumable product.
		private string kProductNameAppleSubscription =  "NA";       // Apple App Store identifier for the subscription product.

		void Start()
		{
//			if (Settings.CloneNumber == 1)
//			{
//				kProductNameAppleConsumable1 =    "com.xbean.Game01.500coins";   
//				kProductNameAppleConsumable2 =    "com.xbean.Game01.3000coins";  
//				kProductNameAppleConsumable3 =    "com.xbean.Game01.6500coins";  
//				kProductNameAppleConsumable4 =    "com.xbean.Game01.14000coins"; 
//				kProductNameAppleNonConsumable = "com.xbean.Game01.removeads";   
//			}
			// If we haven't set up the Unity Purchasing reference
			if (m_StoreController == null)
			{
				// Begin to configure our connection to Purchasing
				InitializePurchasing();
			}
		}

		public void InitializePurchasing() 
		{
			// If we have already connected to Purchasing ...
			if (IsInitialized())
			{
				// ... we are done here.
				return;
			}

			// Create a builder, first passing in a suite of Unity provided stores.
			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

			// Add a product to sell / restore by way of its identifier, associating the general identifier with its store-specific identifiers.
			builder.AddProduct(kProductIDConsumable1, ProductType.Consumable, new IDs(){{ kProductNameAppleConsumable1,       AppleAppStore.Name },{ kProductNameGooglePlayConsumable1,  GooglePlay.Name },});// Continue adding the non-consumable product.
			builder.AddProduct(kProductIDConsumable2, ProductType.Consumable, new IDs(){{ kProductNameAppleConsumable2,       AppleAppStore.Name },{ kProductNameGooglePlayConsumable2,  GooglePlay.Name },});// Continue adding the non-consumable product.
			builder.AddProduct(kProductIDConsumable3, ProductType.Consumable, new IDs(){{ kProductNameAppleConsumable3,       AppleAppStore.Name },{ kProductNameGooglePlayConsumable3,  GooglePlay.Name },});// Continue adding the non-consumable product.
			builder.AddProduct(kProductIDConsumable4, ProductType.Consumable, new IDs(){{ kProductNameAppleConsumable4,       AppleAppStore.Name },{ kProductNameGooglePlayConsumable4,  GooglePlay.Name },});// Continue adding the non-consumable product.
			//builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable, new IDs(){{ kProductNameAppleNonConsumable,       AppleAppStore.Name },{ kProductNameGooglePlayNonConsumable,  GooglePlay.Name },});// And finish adding the subscription product.
			//builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){{ kProductNameAppleSubscription,       AppleAppStore.Name },{ kProductNameGooglePlaySubscription,  GooglePlay.Name },});// Kick off the remainder of the set-up with an asynchrounous call, passing the configuration and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
			UnityPurchasing.Initialize(this, builder);
		}


		private bool IsInitialized()
		{
			// Only say we are initialized if both the Purchasing references are set.
			return m_StoreController != null && m_StoreExtensionProvider != null;
		}


//		public void BuyConsumable1()
//		{
//			// Buy the consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
//			BuyProductID(kProductIDConsumable1);
//		}
//
//		public void BuyConsumable2()
//		{
//			// Buy the consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
//			BuyProductID(kProductIDConsumable2);
//		}
//
//		public void BuyConsumable3()
//		{
//			// Buy the consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
//			BuyProductID(kProductIDConsumable3);
//		}


		public void BuyNonConsumable()
		{
			// Buy the non-consumable product using its general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
			BuyProductID(kProductIDNonConsumable);
		}


		public void BuySubscription()
		{
			// Buy the subscription product using its the general identifier. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
			BuyProductID(kProductIDSubscription);
		}

		public void BuyPackage(CoinPackage package, Action<CoinPackage> callback)
		{
			_purchaseCallback = callback;

			if (package == CoinPackage.Package1)
			{
				BuyProductID(kProductIDConsumable1);
			}
			else if (package == CoinPackage.Package2)
			{
				BuyProductID(kProductIDConsumable2);
			}
			else if (package == CoinPackage.Package3)
			{
				BuyProductID(kProductIDConsumable3);
			}
			else if (package == CoinPackage.Package4)
			{
				BuyProductID(kProductIDConsumable4);
			}
		}

		void BuyProductID(string productId)
		{
			// If the stores throw an unexpected exception, use try..catch to protect my logic here.
			try
			{
				// If Purchasing has been initialized ...
				if (IsInitialized())
				{
					// ... look up the Product reference with the general product identifier and the Purchasing system's products collection.
					Product product = m_StoreController.products.WithID(productId);

					// If the look up found a product for this device's store and that product is ready to be sold ... 
					if (product != null && product.availableToPurchase)
					{
						Debug.Log (string.Format("Purchasing product asychronously: '{0}'", product.definition.id));// ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed asynchronously.
						m_StoreController.InitiatePurchase(product);
					}
					// Otherwise ...
					else
					{
						// ... report the product look-up failure situation  
						Debug.Log ("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
					}
				}
				// Otherwise ...
				else
				{
					// ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or retrying initiailization.
					Debug.Log("BuyProductID FAIL. Not initialized.");
				}
			}
			// Complete the unexpected exception handling ...
			catch (Exception e)
			{
				// ... by reporting any unexpected exception for later diagnosis.
				Debug.Log ("BuyProductID: FAIL. Exception during purchase. " + e);
			}
		}


		// Restore purchases previously made by this customer. Some platforms automatically restore purchases. Apple currently requires explicit purchase restoration for IAP.
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
				// Begin the asynchronous process of restoring purchases. Expect a confirmation response in the Action<bool> below, and ProcessPurchase if there are previously purchased products to restore.
				apple.RestoreTransactions((result) => {
					// The first phase of restoration. If no more responses are received on ProcessPurchase then no purchases are available to be restored.
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
			Debug.Log("OnInitialized: PASS");

			// Overall Purchasing system, configured with products for this application.
			m_StoreController = controller;
			// Store specific subsystem, for accessing device-specific store features.
			m_StoreExtensionProvider = extensions;
		}


		public void OnInitializeFailed(InitializationFailureReason error)
		{
			// Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
			Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
		}


		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) 
		{
			Debug.Log("ID: " + args.purchasedProduct.definition.id + "...");
			// A consumable product has been purchased by this user.
			if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable1, StringComparison.Ordinal))
			{
				// Add small coin here
				if (_purchaseCallback != null)
				{
					_purchaseCallback(CoinPackage.Package1);
				}
			}

			else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable2, StringComparison.Ordinal))
			{
				// Add medium coin here
				if (_purchaseCallback != null)
				{
					_purchaseCallback(CoinPackage.Package2);
				}
			}

			else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable3, StringComparison.Ordinal))
			{
				// Add large coin here
				if (_purchaseCallback != null)
				{
					_purchaseCallback(CoinPackage.Package3);
				}
			}

			else if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable4, StringComparison.Ordinal))
			{
				// Add large coin here
				if (_purchaseCallback != null)
				{
					_purchaseCallback(CoinPackage.Package4);
				}
			}

			// Or ... a non-consumable product has been purchased by this user.
			else if (String.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable, StringComparison.Ordinal))
			{
				Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
			}// Or ... a subscription product has been purchased by this user.
			else if (String.Equals(args.purchasedProduct.definition.id, kProductIDSubscription, StringComparison.Ordinal))
			{
				Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
			}// Or ... an unknown product has been purchased by this user. Fill in additional products here.
			else 
			{
				Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
			}// Return a flag indicating wither this product has completely been received, or if the application needs to be reminded of this purchase at next app launch. Is useful when saving purchased products to the cloud, and when that save is delayed.
			return PurchaseProcessingResult.Complete;
		}


		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			// A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing this reason with the user.
			Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}",product.definition.storeSpecificId, failureReason));
		}
	}
}