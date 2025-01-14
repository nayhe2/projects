import React, { useState, useEffect } from "react";
import { useSession, useSupabaseClient, useSessionContext } from '@supabase/auth-helpers-react';
import Navbar from "./Navbar";
import Instance from "./Instance";

export default function App() {

    const [concerts, setConcerts] = useState([]);
    const [name, setName] = useState("");
    const [artistImages, setArtistImages] = useState({});
    const session = useSession();
    const supabase = useSupabaseClient();
    const { isLoading } = useSessionContext();

    const googleSignIn = async () => {
        const { error } = await supabase.auth.signInWithOAuth({
            provider: 'google',
            options: {
                scopes: 'https://www.googleapis.com/auth/calendar'
            }
        });
        if (error) {
            alert("Error logging in to Google provider with Supabase");
            console.error(error);
        }
    };

    const signOut = async () => {
        await supabase.auth.signOut();
    };
    
    const fetchArtistImage = async (artistName) => {
        try {
            const tokenResponse = await fetch("http://localhost:5175/api/spotify-token");
            const tokenData = await tokenResponse.json();
            if (!tokenData.accessToken) {
                throw new Error('No access token found');
            }
    
            const token = tokenData.accessToken;
            console.log(artistName)
            const response = await fetch(
                `https://api.spotify.com/v1/search?q=${encodeURIComponent(artistName)}&type=artist&limit=1`,
                {
                    headers: {
                        Authorization: `Bearer ${token}`,
                    },
                }
            );
    
            if (!response.ok) {
                throw new Error("Failed to fetch artist image");
            }
    
            const data = await response.json();
            const artist = data.artists.items[0];
            return artist?.images?.[0]?.url || null;
        } catch (error) {
            console.error(`Error fetching image for ${artistName}:`, error);
            return null;
        }
    };
    

    const fetchConcerts = async () => {
        console.log("name:",name)
        try {
            const response = await fetch(`http://localhost:5175/api/concerts?artist=${encodeURIComponent(name)}`);
     
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
     
            const data = await response.json();
            if (data._embedded?.events) {
                const events = data._embedded.events;
                console.log(events)
                const imagesWithConcerts = await Promise.all(
                    events.map(async (event) => {
                        const artistName = event.name;
                        const image = await fetchArtistImage(artistName);
                        return { ...event, artistImage: image };
                    })
                );
            setConcerts(imagesWithConcerts);
            } 
            else {
                setConcerts([]);
            }
        } catch (error) {
            console.error("Error fetching concert data:", error);
        }
    };
    
    useEffect(() => {
    if (!name) return;

    fetchConcerts();
    }, [name]);


    if (isLoading) {
        return <div className="flex justify-center items-center h-screen bg-black text-white">Loading...</div>;
    }

    const concertElements = concerts.map((item) => (
        <Instance
            key={item.id}
            name={item.name}
            date={item.dates.start}
            price={item.priceRanges && item.priceRanges.length > 0 ? item.priceRanges[0] : "N/A"}
            location={item._embedded?.venues?.[0]}
            artistImage={item.artistImage}
            status={item.dates.status.code}
        />
    ));

    return (
        <div className="min-h-screen bg-black">
            <div className="container mx-auto px-4">
                <div className="flex justify-end p-4">
                    {session ? (
                        <div className="flex items-center gap-4">
                            <span className="text-white">
                                {session.user.email}
                            </span>
                            <button onClick={signOut} className="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded">Sign Out</button>
                        </div>
                    ) : (
                        <button onClick={googleSignIn} className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded">Sign in with Google</button>
                    )}
                </div>
                <Navbar setName={setName} />
                <div>{concertElements}</div>
            </div>
        </div>
    );
}
