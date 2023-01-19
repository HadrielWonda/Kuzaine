﻿using Kuzaine.Domain;
using Kuzaine.Domain.Enums;
using Kuzaine.Helpers;
using Kuzaine.Services;



namespace Kuzaine.Builders.NextJs.Domain.Features;

public class NextJsEntityListTableBuilder
{
    private readonly IKuzaineUtilities _utilities;

    public NextJsEntityListTableBuilder(IKuzaineUtilities utilities)
    {
        _utilities = utilities;
    }

    public void CreateFile(string nextSrc, string entityName, string entityPlural, List<NextJsEntityProperty> properties)
    {
        var routesIndexClassPath = ClassPathHelper.NextJsSpaFeatureClassPath(nextSrc,
            entityPlural,
            NextJsDomainCategory.Features,
            $"{FileNames.NextJsEntityFeatureListTableName(entityName)}.tsx");
        var routesIndexFileText = GetFileText(entityName, entityPlural, properties);
        _utilities.CreateFile(routesIndexClassPath, routesIndexFileText);
    }

    public static string GetFileText(string entityName, string entityPlural, List<NextJsEntityProperty> properties)
    {
        var readDtoName = FileNames.GetDtoName(entityName, Dto.Read);
        var entityPluralLowercase = entityPlural.ToLower();
        var entityPluralUpperFirst = entityPlural.UppercaseFirstLetter();
        var entityUpperFirst = entityName.UppercaseFirstLetter();
        var entityPluralLowercaseFirst = entityPlural.LowercaseFirstLetter();
        var entityLowerFirst = entityName.LowercaseFirstLetter();
        var deletePermission = FeatureType.DeleteRecord.DefaultPermission(entityPlural);
        var updatePermission = FeatureType.UpdateRecord.DefaultPermission(entityPlural);

        string columnHelpers = GetColumnHelpers(properties);

        return @$"import {{
  Checkbox,
  PaginatedTable,
  TrashButton,
  usePaginatedTableContext,
}} from ""@/components/forms"";
import {{ useHasPermission }} from ""@/domain/permissions"";
import useDeleteModal from ""@/components/modal/ConfirmDeleteModal"";
import {{ Notifications }} from ""@/components/notifications"";
import {{ {readDtoName}, useDelete{entityUpperFirst}, use{entityPluralUpperFirst} }} from ""@/domain/{entityPluralLowercaseFirst}"";
import ""@tanstack/react-table"";
import {{ createColumnHelper, SortingState }} from ""@tanstack/react-table"";
import {{ useRouter }} from ""next/router"";

interface {entityUpperFirst}ListTableProps {{
  queryFilter?: string | undefined;
}}

export function {entityUpperFirst}ListTable({{ queryFilter }}: {entityUpperFirst}ListTableProps) {{
  const router = useRouter();
  const {{ sorting, pageSize, pageNumber }} = usePaginatedTableContext();
  const {deletePermission.LowercaseFirstLetter()} = useHasPermission(""{deletePermission}"");
  const {updatePermission.LowercaseFirstLetter()} = useHasPermission(""{updatePermission}"");

  const onRowClick = {updatePermission.LowercaseFirstLetter()}.hasPermission
    ? (row) => router.push(`/{entityPluralLowercase}/${{row.id}}`)
    : undefined;

  const openDeleteModal = useDeleteModal();
  const delete{entityUpperFirst}Api = useDelete{entityUpperFirst}();
  function delete{entityUpperFirst}(id: string) {{
    delete{entityUpperFirst}Api
      .mutateAsync(id)
      .then(() => {{
        Notifications.success(""{entityUpperFirst} deleted successfully"");
      }})
      .catch((e) => {{
        Notifications.error(""There was an error deleting the {entityLowerFirst}"");
        console.error(e);
      }});
  }}

  const {{ data: {entityLowerFirst}Response, isLoading }} = use{entityPluralUpperFirst}({{
    sortOrder: sorting as SortingState,
    pageSize,
    pageNumber,
    filters: queryFilter,
    hasArtificialDelay: true,
  }});
  const {entityLowerFirst}Data = {entityLowerFirst}Response?.data;
  const {entityLowerFirst}Pagination = {entityLowerFirst}Response?.pagination;

  const columnHelper = createColumnHelper<{readDtoName}>();
  const columns = [{columnHelpers}
    columnHelper.accessor(""id"", {{
      enableSorting: false,
      meta: {{ thClassName: ""w-10"" }},
      cell: (row) => (
        <div className=""flex items-center justify-center w-full"">
          {{{deletePermission.LowercaseFirstLetter()}.hasPermission && (
            <TrashButton
              onClick={{(e) => {{
                openDeleteModal({{
                  onConfirm: () => delete{entityUpperFirst}(row.getValue()),
                }});
                e.stopPropagation();
              }}}}
            />
          )}}
        </div>
      ),
      header: () => <span className=""""></span>,
    }}),
  ];

  return (
    <PaginatedTable
      data={{{entityLowerFirst}Data}}
      columns={{columns}}
      apiPagination={{{entityLowerFirst}Pagination}}
      entityPlural=""{entityPluralUpperFirst}""
      isLoading={{isLoading}}
      onRowClick={{onRowClick}}
    />
  );
}}";
    }

    private static string GetColumnHelpers(List<NextJsEntityProperty> properties)
    {
        var columnHelpers = "";
        foreach (var property in properties)
        {
            columnHelpers += property.TypeEnum.ColumnHelperText(property.Name, property.Label);
        }

        return columnHelpers;
    }
}
