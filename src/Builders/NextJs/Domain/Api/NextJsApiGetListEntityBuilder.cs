﻿using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;



namespace Kuzaine.Builders.NextJs.Domain.Api;

public class NextJsApiGetListEntityBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public NextJsApiGetListEntityBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateApiFile(string spaDirectory, string entityName, string entityPlural, string clientName)
    {
        var routesIndexClassPath = ClassPathHelper.NextJsSpaFeatureClassPath(spaDirectory,
            entityPlural,
            NextJsDomainCategory.Api,
            $"{FeatureType.GetList.NextJsApiName(entityName)}.tsx");
        var routesIndexFileText = GetApiText(entityName, entityPlural, clientName);
        _utilities.CreateFile(routesIndexClassPath, routesIndexFileText);
    }

    public static string GetApiText(string entityName, string entityPlural, string clientName)
    {
        var readDtoName = FileNames.GetDtoName(entityName, Dto.Read);
        var entityPluralUppercaseFirst = entityPlural.UppercaseFirstLetter();
        var entityPluralLowercase = entityPlural.ToLower();
        var entityPluralLowercaseFirst = entityPlural.LowercaseFirstLetter();
        var entityNameLowercase = entityName.ToLower();
        var keysImport = FileNames.NextJsApiKeysFilename(entityName);
        var keyExportName = FileNames.NextJsApiKeysExport(entityName);

        return @$"import {{ clients }} from ""@/lib/axios"";
import {{ PagedResponse, Pagination }} from ""@/types/apis"";
import {{ generateSieveSortOrder }} from ""@/utils/sorting"";
import {{ AxiosResponse }} from ""axios"";
import queryString from ""query-string"";
import {{ useQuery }} from ""react-query"";
import {{ QueryParams, {readDtoName}, {keyExportName} }} from ""@/domain/{entityPluralLowercaseFirst}"";

interface delayProps {{
  hasArtificialDelay?: boolean;
  delayInMs?: number;
}}

interface {entityNameLowercase}ListApiProps extends delayProps {{
  queryString: string;
}}
const get{entityPluralUppercaseFirst} = async ({{
  queryString,
  hasArtificialDelay,
  delayInMs,
}}: {entityNameLowercase}ListApiProps) => {{
  queryString = queryString == """" ? queryString : `?${{queryString}}`;

  delayInMs = hasArtificialDelay ? delayInMs : 0;

  const [json] = await Promise.all([
    clients.{clientName}().then((axios) =>
      axios
        .get(`/{entityPluralLowercase}${{queryString}}`)
        .then((response: AxiosResponse<{readDtoName}[]>) => {{
          return {{
            data: response.data as {readDtoName}[],
            pagination: JSON.parse(
              response.headers[""x-pagination""] ?? """"
            ) as Pagination,
          }} as PagedResponse<{readDtoName}>;
        }})
    ),
    new Promise((resolve) => setTimeout(resolve, delayInMs)),
  ]);
  return json;
}};

interface {entityNameLowercase}ListHookProps extends QueryParams, delayProps {{}}
export const use{entityPluralUppercaseFirst} = ({{
  pageNumber,
  pageSize,
  filters,
  sortOrder,
  hasArtificialDelay = false,
  delayInMs = 500,
}}: {entityNameLowercase}ListHookProps) => {{
  let sortOrderString = generateSieveSortOrder(sortOrder);
  let queryParams = queryString.stringify({{
    pageNumber,
    pageSize,
    filters,
    sortOrder: sortOrderString,
  }});

  return useQuery({keyExportName}.list(queryParams ?? """"), () =>
    get{entityPluralUppercaseFirst}({{ queryString: queryParams, hasArtificialDelay, delayInMs }})
  );
}};";
    }
}
