import {setUnreadGroupsHook} from "../NavMenu";

export default function GroupSidebarComponent(props) {
  return (
      <div className="relative flex flex-col items-center group" onClick={() => setUnreadGroupsHook(props.id, true)}>
        <a
            href={`/group/${props.id}`}
            className="flex flex-col items-center rounded-full h-[50px] w-[50px]"
        >
          <img src={props.img} alt="group image" className="rounded-full h-full w-full" />

          {props.unread && (
              <span className="absolute top-0 right-0 h-3 w-3 bg-red-500 rounded-full border-2 border-white"></span>
          )}
          
        </a>
        <span className="absolute left-[60px] bg-gray-700 text-white text-xs rounded-md px-2 py-1 opacity-0 group-hover:opacity-100 transition-opacity duration-300 z-50">
          {props.name}
        </span>
      </div>
  );
}