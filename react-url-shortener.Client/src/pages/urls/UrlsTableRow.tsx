import { UrlResponse } from "@src/models/Url";

type UrlsTableRowProps = {
  url: UrlResponse;
  columns: string[];
  isHighlighted: boolean;
  wrapper?: (index: number, content: React.ReactNode) => React.ReactNode;
};

const UrlsTableRow = ({ url, columns, isHighlighted, wrapper }: UrlsTableRowProps) => (
  <tr className={isHighlighted ? "bg-slate-300" : ""}>
    {columns.map((column, index) => {
      const value = url[column as keyof UrlResponse] as string;
      const content = wrapper?.(index, value) ?? value;

      return (
        <td
          key={index}
          className="p-2 break-words max-w-[300px] text-sm leading-6 font-medium text-gray-900"
        >
          {content}
        </td>
      );
    })}
  </tr>
);

export default UrlsTableRow;
