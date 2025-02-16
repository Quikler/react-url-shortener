import UrlsTableHeader from "@src/pages/urls/UrlsTableHeader";
import UrlsTableRow from "@src/pages/urls/UrlsTableRow";
import { useAuth } from "@src/hooks/useAuth";
import { UrlResponse } from "@src/models/Url";
import { Link } from "react-router-dom";

type UrlsTableProps = {
  urls: UrlResponse[];
};

const UrlsTable = ({ urls }: UrlsTableProps) => {
  const { user } = useAuth();

  return (
    <table className="rounded-xl">
      <UrlsTableHeader columns={["Id", "Original URL", "Short URL"]} />
      <tbody className="divide-y-1 divide-black-300">
        {urls.map((url) => (
          <UrlsTableRow
            key={url.id}
            url={url}
            columns={["id", "urlOriginal", "urlShortened"]}
            isHighlighted={user?.id === url.userId}
            wrapper={(index, content) =>
              index === 2 ? (
                <Link to={url.urlShortened} target="_blank" className="px-2 py-1 bg-blue-500 text-white rounded">{content}</Link>
              ) : (
                content
              )
            }
          />
        ))}
      </tbody>
    </table>
  );
};

export default UrlsTable;
