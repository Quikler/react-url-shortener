type UrlsTableHeaderProps = {
  columns: string[];
};

const UrlsTableHeader = ({ columns }: UrlsTableHeaderProps) => (
  <thead>
    <tr>
      {columns.map((column, index) => (
        <th
          key={index}
          scope="col"
          className={`p-5 text-left text-sm leading-6 font-semibold capitalize`}
        >
          {column}
        </th>
      ))}
    </tr>
  </thead>
);

export default UrlsTableHeader;
