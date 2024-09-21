export const Message = (props) => {
  return (
      <div className="mb-2 relative">
        <div className="font-bold text-gray-300">{props.sender.username}
          <span className="text-gray-800 text-xs ml-2">
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
  )
}
