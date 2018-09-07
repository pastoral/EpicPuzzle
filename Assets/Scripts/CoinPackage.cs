using UnityEngine;

public enum CoinPackage
{
	Package1,
	Package2,
	Package3,
	Package4
}

public static class CoinPackageHelper
{
	public static int GetCoins(this CoinPackage package)
	{
		if (package == CoinPackage.Package1)
		{
			return 500;
		}
		
		if (package == CoinPackage.Package2)
		{
			return 3000;
		}
		
		if (package == CoinPackage.Package3)
		{
			return 6500;
		}

		if (package == CoinPackage.Package4)
		{
			return 14000;
		}

		return 0;
	}
}
