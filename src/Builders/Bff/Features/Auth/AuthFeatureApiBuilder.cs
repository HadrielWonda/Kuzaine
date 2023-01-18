using Domain.Enums;
using Helpers;
using Services;


namespace Kuzaine.Builders.Bff.Features.Auth;

public class AuthFeatureApiBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public AuthFeatureApiBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateAuthFeatureApis(string spaDirectory)
    {
        var routesIndexClassPath = ClassPathHelper.BffSpaFeatureClassPath(spaDirectory, "Auth", BffFeatureCategory.Api, "index.ts");
        var routesIndexFileText = GetAuthFeatureApisIndexText();
        _utilities.CreateFile(routesIndexClassPath, routesIndexFileText);

        var routesLoginClassPath = ClassPathHelper.BffSpaFeatureClassPath(spaDirectory, "Auth", BffFeatureCategory.Api, "useAuthUser.tsx");
        var routesLoginFileText = GetAuthFeatureApisLoginText();
        _utilities.CreateFile(routesLoginClassPath, routesLoginFileText);
    }

    public static string GetAuthFeatureApisIndexText()
    {
        return @$"export * from './useAuthUser';";
    }

    public static string GetAuthFeatureApisLoginText()
    {
        return @$"import {{ api }} from '@/lib/axios';
import {{ useEffect, useState }} from 'react';
import {{ useQuery }} from 'react-query';

const claimsKeys = {{
	claim: ['claims'],
}};

const fetchClaims = async () => api.get('/bff/user').then((res) => res.data);

function useClaims() {{
	return useQuery(
		claimsKeys.claim,
		async () => {{
			return fetchClaims();
		}},
		{{
			retry: false,
		}}
	);
}}

function useAuthUser() {{
	const {{ data: claims, isLoading }} = useClaims();

	let logoutUrl = claims?.find((claim: any) => claim.type === 'bff:logout_url');
	let nameDict =
		claims?.find((claim: any) => claim.type === 'name') ||
		claims?.find((claim: any) => claim.type === 'sub');
	let username = nameDict?.value;

	const [isLoggedIn, setIsLoggedIn] = useState(false);
	useEffect(() => {{
		setIsLoggedIn(!!username);
	}}, [username]);

	return {{
		username,
		logoutUrl,
		isLoading,
		isLoggedIn,
	}};
}}

export {{ useAuthUser }};
";
    }
}
