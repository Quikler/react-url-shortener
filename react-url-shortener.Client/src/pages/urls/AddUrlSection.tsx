import Button from "@src/components/ui/Buttons/Button";
import ErrorMessage from "@src/components/ui/ErrorMessage";
import Input from "@src/components/ui/Inputs/Input";
import { useToast } from "@src/hooks/useToast";
import { handleError } from "@src/utils/helpers";
import { useForm } from "react-hook-form";
import { twMerge } from "tailwind-merge";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";

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

  const { danger, success } = useToast();

  const handleAddUrlSubmit = handleSubmit(async (data) => {
    try {
      await UrlsShortenerService.create(data.url);
      success("Url created successfully");
    } catch (e: any) {
      handleError(e, danger);
    }
  });

  return (
    <div
      {...rest}
      className={twMerge("w-2xl p-8 bg-gradient-to-r dark:from-gray-700 dark:to-gray-900 from-gray-200 to-gray-200 shadow rounded-2xl", className)}
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
