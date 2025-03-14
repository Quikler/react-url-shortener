import getVariant from "@src/utils/helpers";

const getButtonVariant = (variant: string) => {
  const v = getVariant(
    [
      {
        key: "primary",
        style: "btn-primary",
      },
      {
        key: "secondary",
        style: "btn-secondary",
      },
      {
        key: "danger",
        style: "btn-danger",
      },
      {
        key: "info",
        style: "btn-info",
      },
    ],
    variant,
    "btn-default"
  );

  return v;
};

export default getButtonVariant;
