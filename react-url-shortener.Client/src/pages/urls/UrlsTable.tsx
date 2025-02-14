const UrlsTable = () => {
  return (
    <table className="rounded-xl">
      <thead>
        <tr className="bg-gray-50">
          <th
            scope="col"
            className="p-5 text-left text-sm leading-6 font-semibold text-gray-900 capitalize"
          >
            Company
          </th>
          <th
            scope="col"
            className="p-5 text-left text-sm leading-6 font-semibold text-gray-900 capitalize"
          >
            User ID
          </th>
          <th
            scope="col"
            className="p-5 text-left text-sm leading-6 font-semibold text-gray-900 capitalize"
          >
            Type
          </th>
          <th
            scope="col"
            className="p-5 text-left text-sm leading-6 font-semibold text-gray-900 capitalize"
          >
            Industry Type
          </th>
        </tr>
      </thead>
      <tbody className="divide-y divide-gray-300 ">
        <tr>
          <td className="p-5 whitespace-nowrap text-sm leading-6 font-medium text-gray-900 ">
            Louis Vuitton
          </td>
          <td className="p-5 whitespace-nowrap text-sm leading-6 font-medium text-gray-900">
            20010510
          </td>
          <td className="p-5 whitespace-nowrap text-sm leading-6 font-medium text-gray-900">
            Customer
          </td>
          <td className="p-5 whitespace-nowrap text-sm leading-6 font-medium text-gray-900">
            Accessories
          </td>
        </tr>
      </tbody>
    </table>
  );
};

export default UrlsTable;
