import { useEffect, useState } from "react";
import { setUnreadGroupsHook } from "../NavMenu";
import {cacheImage, getCachedImage} from "../../helpers/CacheHelper";

export default function GroupSidebarComponent(props) {
  const [cachedImageUrl, setCachedImageUrl] = useState(null);

  const isValidImageUrl = (url) => url && url.trim() !== "";

  const processName = (name) => {
    if (name.includes(" ")) {
      return name
          .split(" ")
          .map((word) => word.charAt(0).toUpperCase())
          .join("");
    }

    if (name.length > 6) {
      return name.slice(0, 6);
    }

    return name;
  };

  const displayedName = processName(props.name);

  useEffect(() => {
    const fetchAndCacheImage = async () => {
      if (isValidImageUrl(props.imageUrl)) {
        const imageUrl = `${window.location.origin}/api/cdn/${props.imageUrl}`;
        const cachedUrl = await getCachedImage(imageUrl);
        if (cachedUrl) {
          setCachedImageUrl(cachedUrl);
        } else {
          await cacheImage(imageUrl);
          setCachedImageUrl(imageUrl);
        }
      }
    };

    fetchAndCacheImage();
  }, [props.imageUrl, props.id]);

  return (
      <div className="relative flex flex-col items-center group" onClick={() => setUnreadGroupsHook(props.id, true)}>
        <a
            href={`/group/${props.id}`}
            className="flex flex-col items-center rounded-full h-[50px] w-[50px]"
        >
          {cachedImageUrl ? (
              <img
                  src={cachedImageUrl}
                  alt="group image"
                  className="rounded-full h-full w-full bg-MainDark"
              />
          ) : (
              <div className="rounded-full h-full w-full bg-MainDark flex items-center justify-center text-white text-sm overflow-hidden whitespace-nowrap text-ellipsis">
                {displayedName}
              </div>
          )}

          {props.unread && (
              <span className="absolute top-0 right-0 h-3 w-3 bg-red-500 rounded-full border-2 border-white"></span>
          )}
        </a>
        <span className="absolute left-[60px] bg-gray-700 text-white text-xs rounded-md px-2 py-1 opacity-0 group-hover:opacity-100 transition-opacity duration-300 z-50 whitespace-nowrap">
        {props.name}
      </span>
      </div>
  );
}