import React from "react";
import { useSession } from '@supabase/auth-helpers-react';
import DefaultImage from "./assets/pfp1.png";

function Instance({ name, date, price, location, artistImage,status }) {
    const session = useSession();
    const profPic = artistImage ? artistImage : DefaultImage;

    const createCalendarEvent = async () => {
        if (!session?.provider_token) {
            alert("Please sign in with Google to add events to your calendar");
            return;
        }
    
        let description = `Concert: ${name}\n`;
        if (location.name) description += `Place: ${location.name}\n`;
        if (location.address?.line1) description += `Address: ${location.address.line1}\n`;
        if (location.city?.name) description += `City: ${location.city.name}\n`;
        if (location.country?.name) description += `Country: ${location.country.name}\n`;
        if (location.state?.name) description += `State: ${location.state.name}\n`;
        if (location.postalCode != undefined) description += `Postal Code: ${location.postalCode}\n`;
        if (price.min !== undefined && price.max !== price.min) description += `Price Range: ${price.min} - ${price.max} ${price.currency}\n`;
        if (price.min !== undefined && price.max === price.min) description += `Price: ${price.min} ${price.currency}\n`;

        const startDateTime = new Date(`${date.localDate}T${date.localTime}`);

        const endDateTime = new Date(startDateTime);
        endDateTime.setHours(startDateTime.getHours() + 3);


        const event = {
            'summary': name,
            'description': description,
            'start': {
                'dateTime': startDateTime.toISOString(),
                'timeZone': Intl.DateTimeFormat().resolvedOptions().timeZone
            },
            'end': {
                'dateTime': endDateTime.toISOString(),
                'timeZone': Intl.DateTimeFormat().resolvedOptions().timeZone
            },
            'location': location.address?.line1 ? 
                `${location.address.line1}, ${location.city?.name || ''}, ${location.state?.name || ''} ${location.postalCode || ''}` 
                : location.name
        };

        try {
            const response = await fetch("http://localhost:5175/api/calendar-event", {
                method: "POST",
                headers: {
                    'Authorization': `Bearer ${session.provider_token}`,
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(event),
            });
    
            if (!response.ok) {
                throw new Error('Failed to create calendar event');
            }
    
            alert("Event added to Google Calendar!");
        } catch (error) {
            console.error("Error creating calendar event:", error);
        }
    };
    

    return (
        <div className="flex box-sh border-b-2 my-5 justify-center max-w-3xl mx-auto bg-black text-white">
            <div className="flex-col relative w-1/2">
                <div className="w-full h-61">
                    <img className="w-full h-full object-cover" src={profPic} alt="artist profile" />
                </div>
                <div className="absolute bottom-4 left-4 bg-black bg-opacity-80 p-2 rounded">
                    {name}
                </div>
            </div>
            <div className="flex-col p-5 w-1/2">
                {date.localDate && <p>{date.localDate}</p>}
                {date.localTime && <p>{date.localTime.slice(0,5)}</p>}

                {location != undefined && (
                <>
                    {location.name && <p>{location.name}</p>}
                    {location.country?.name && <p>{location.country.name}</p>}
                    {location.state?.name && <p>{location.state.name}</p>}
                    {location.city?.name && <p>{location.city.name}</p>}
                    {location.address?.line1 && <p>{location.address.line1}</p>}
                    {location.postalCode && <p>{location.postalCode}</p>}
                </>
                )}

                {price.min !== undefined && price.max !== price.min && (
                    <p>{price.min} - {price.max} {price.currency}</p>
                )}
                {price.min !== undefined && price.max === price.min && (
                    <p>{price.min} {price.currency}</p>
                )}
                <div className={status === "onsale" ? "text-green-400" : "text-red-400"}>
                    {status=="onsale" ? "scheduled": "canceled"}
                </div>
                
                {session && location?.address?.line1 && status === "onsale" && (
                    <button
                        onClick={createCalendarEvent}
                        className="mt-4 bg-green-600 hover:bg-green-700 text-white px-4 py-2 rounded transition-colors duration-200"
                    >
                        Add to Calendar
                    </button>
                )}
            </div>
        </div>
    );
}

export default Instance;