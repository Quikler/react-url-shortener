import Button from "@src/components/ui/Button";
import ErrorMessage from "@src/components/ui/ErrorMessage";
import Input from "@src/components/ui/Input";
import { UrlResponse } from "@src/models/Url";
import { UrlsShortenerService } from "@src/services/api/UrlsShortenerService";
import { useForm } from "react-hook-form";
import { twMerge } from "tailwind-merge";

type AddUrlSectionProps = React.HTMLAttributes<HTMLDivElement> & {
  onUrlCreated: (data: UrlResponse) => void;
};

const AddUrlSection = ({ onUrlCreated, className, ...rest }: AddUrlSectionProps) => {
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

  const handleAddUrlSubmit = handleSubmit(async (data) => {
    try {
      const urlResponse = await UrlsShortenerService.create(data.url);
      onUrlCreated(urlResponse);
    } catch (e: any) {
      console.error("Unable to create url:", e.message);
    }
  });

  return (
    <>
      <div {...rest} className={twMerge("w-full", className)}>
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
    </>
  );
};

export default AddUrlSection;
