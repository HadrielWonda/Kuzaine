using Helpers;
using Services;


namespace Kuzaine.Builders.Bff.Src;

public class TypesBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public TypesBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateApiTypes(string spaDirectory)
    {
        var classPath = ClassPathHelper.BffSpaSrcApiTypesClassPath(spaDirectory, "index.ts");
        var fileText = GetApiTypesText();
        _utilities.CreateFile(classPath, fileText);
    }

    public static string GetApiTypesText()
    {
        return @$"export interface PagedResponse<T> {{
  pagination: Pagination;
  data: T[];
}}

export interface Pagination {{
  currentEndIndex: number;
  currentPageSize: number;
  currentStartIndex: number;
  hasNext: boolean;
  hasPrevious: boolean;
  pageNumber: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}}";
    }
}