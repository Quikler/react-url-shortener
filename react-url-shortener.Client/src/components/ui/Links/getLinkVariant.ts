import { getVariant } from "@src/utils/helpers";
import "./Links.css"

const getLinkVariant = (variant: string) => {
  const v = getVariant(
    [
      {
        key: "primary",
        style: "link-primary",
      },
      {
        key: "secondary",
        style: "link-secondary",
      },
    ],
    variant,
    "link-default"
  );

  return v;
};

export default getLinkVariant;
