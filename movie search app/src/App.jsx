import React from 'react'
import { useState } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'
import Movie from './components/Movie'
import Navbar from './components/Navbar'

export default function App() {
  const [movieList, setMovieList] = React.useState([]);
  const [inputValue, setInputValue] = React.useState("")

  const getMovie = () => {
    fetch(`https://api.themoviedb.org/3/search/movie?api_key=a94ec6a7c66183559d48b43ac2cd2d9d&query=${inputValue}`)
      .then((res) => res.json())
      .then((json) => {
        //console.log(json);
        console.log(json.results); 
        setMovieList(json.results);
      })
      .catch((error) => console.error("Error fetching movies:", error));
  };
  
  React.useEffect(() => {
    getMovie();
  }, [inputValue]);

  const movies = movieList.map((item) => (
    <Movie 
      key={item.id} 
      org_title={item.original_title}
      img={item.poster_path}
      releaseDate={item.release_date}
      overview={item.overview}
    />
  ));

  const handleInputChange = (value) =>{
    setInputValue(value);
  }

  console.log(movieList)
  return (
    <div className="min-h-screen bg-gray-900">
      <Navbar onInputChange={handleInputChange}/>
      <div className='bg-gray-900 flex flex-wrap justify-center items-center'>
        {movies}
      </div>
  </div>
  )
}


