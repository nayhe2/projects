import React from "react";

export default function Movie(props) {
    //const fallbackImage = "https://via.placeholder.com/500";
    const fallbackImage = "/fallbackImage.jpg";

    return (
      <div className="flex flex-col justify-center items-center bg-black text-white m-2 w-64 rounded">
        <div className="text-lg h-16 overflow-hidden text-ellipsis line-clamp-2">
          {props.org_title}
        </div>
        <div className="w-full h-64 overflow-hidden relative group">
          <img
            className="object-cover w-full h-full group-hover:scale-110 transition-transform duration-300"
            src={props.img ? `https://image.tmdb.org/t/p/original/${props.img}` : fallbackImage}
            alt="movie poster"
          />
          <div className="text-sm absolute inset-0 bg-black bg-opacity-75 opacity-0 hover:opacity-100 transition-opacity duration-300 flex flex-col p-4">
          <div 
            className="overflow-y-auto h-full pr-2 hover:pr-0 transition-all"
            style={{
              scrollbarWidth: 'thin',
              scrollbarColor: 'gray transparent',
            }}
          >
              <p className="text-white">
                {props.overview || "No description available"}
              </p>
            </div>
          </div>
        </div>
        <div>
          {props.releaseDate}
        </div>
      </div>
    );
  }