export const Message = (props) => {
  return (
      <div className="mb-2 relative flex items-start">
        <img
            src={`/api/cdn/${props.sender.imageUrl}`}
            alt={props.sender.username}
            className="w-10 h-10 rounded-full mr-3"
        />
        <div>
          <div className="font-bold text-gray-300">
            {props.sender.username}
            <span className="text-gray-600 text-xs ml-2">
            {
              new Date(props.createdAt * 1000).toLocaleDateString([], {
                year: 'numeric',
                month: '2-digit',
                day: '2-digit',
                hour: '2-digit',
                minute: '2-digit',
              })
            }
          </span>
          </div>
          <div className="text-gray-100 text-sm">{props.content}</div>
        </div>
      </div>
  );
};
