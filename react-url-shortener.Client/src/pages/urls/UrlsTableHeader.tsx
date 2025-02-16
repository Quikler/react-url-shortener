type UrlsTableHeaderProps = {
  columns: string[];
};

const UrlsTableHeader = ({ columns }: UrlsTableHeaderProps) => (
  <thead>
    <tr className="bg-gray-50">
      {columns.map((column) => (
        <th
          key={column}
          scope="col"
          className="p-5 text-left text-sm leading-6 font-semibold text-gray-900 capitalize"
        >
          {column}
        </th>
      ))}
    </tr>
  </thead>
);

export default UrlsTableHeader;
