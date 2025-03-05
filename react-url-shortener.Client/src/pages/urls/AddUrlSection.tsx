import Button from "@src/components/ui/Button";
import ErrorMessage from "@src/components/ui/ErrorMessage";
import Input from "@src/components/ui/Input";
import { useToast } from "@src/hooks/useToast";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import { handleError } from "@src/utils/helpers";
import { useForm } from "react-hook-form";
import { twMerge } from "tailwind-merge";
import { useUrls } from "./UrlsContext";

type AddUrlSectionProps = React.HTMLAttributes<HTMLDivElement> & {};

const AddUrlSection = ({ className, ...rest }: AddUrlSectionProps) => {
  const {
    handleSubmit,
    register,
    formState: { errors },
  } = useForm({
    mode: "onTouched",
    defaultValues: {
      url: "",
    },
  });

  const { createUrl } = useUrls();
  const { danger, success } = useToast();

  const handleAddUrlSubmit = handleSubmit(async (data) => {
    try {
      const urlResponse = await UrlsShortenerService.create(data.url);
      if (urlResponse) {
        createUrl(urlResponse);
        success("Url created successfully");
      }
    } catch (e: any) {
      handleError(e, danger);
    }
  });

  return (
    <div
      {...rest}
      className={twMerge("w-2xl p-8 bg-white shadow rounded-2xl border-black", className)}
    >
      <form onSubmit={handleAddUrlSubmit} className="flex flex-col gap-3">
        <p className="text-3xl text-left">Add new url</p>
        <Input
          {...register("url", {
            required: "Url is required",
            validate: (url) =>
              url.startsWith("https://") ||
              url.startsWith("http://") ||
              "Url should start with http or https protocol",
          })}
          placeholder="Enter url (http://example.com)"
          className="w-full"
        />
        <ErrorMessage>{errors.url?.message}</ErrorMessage>
        <Button type="submit" className="self-start">
          Submit
        </Button>
      </form>
    </div>
  );
};

export default AddUrlSection;
