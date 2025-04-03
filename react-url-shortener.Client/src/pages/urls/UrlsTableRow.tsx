import { UrlResponse } from "@src/services/api/models/Url";

type UrlsTableRowProps = {
  url: UrlResponse;
  columns: string[];
  isHighlighted: boolean;
  wrapper?: (index: number, content: React.ReactNode) => React.ReactNode;
};

const UrlsTableRow = ({ url, columns, isHighlighted, wrapper }: UrlsTableRowProps) => {
  return (
    <tr className={`border-none ${isHighlighted ? 'dark:bg-gray-500/40 bg-gray-400/40' : 'bg-transparent'}`}>
      {columns.map((column, index) => {
        const value = url[column as keyof UrlResponse] as string;
        const content = wrapper?.(index, value) ?? value;

        return (
          <td
            key={index}
            className="overflow-hidden p-2 break-words text-sm leading-6 font-medium"
          >
            {content}
          </td>
        );
      })}
    </tr>
  );
};

export default UrlsTableRow;
