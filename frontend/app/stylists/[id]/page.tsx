"use client";
import { useEffect, useState } from "react";
import {useParams, useRouter} from "next/navigation";

interface Stylist {
    id: number;
    fullName: string;
    specialties: string[];
    rating: number;
    location: string;
    serviceIds: string[];
}

interface Service {
    id: number;
    name: string;
    price: number;
    durationMinutes: number;
}

export default function StylistProfilePage(){
    const params = useParams();
    const router = useRouter();
    
    const [stylist, setStylist] = useState<Stylist | null>(null);
    const [services, setServices] = useState<Service[]>([]);
    const [loading, setLoading] = useState(true);
    useEffect(() => {
        async function fetchStylistProfile(){
            try{
                // fetch stylist details
                const stylistResponse = await fetch(`http://localhost:5170/api/stylists/${params.id}`);
                if (!stylistResponse.ok) {
                    throw new Error(`HTTP error fetching stylist! status: ${stylistResponse.status}; message: ${stylistResponse.statusText}, body: ${await stylistResponse.text()}`);
                }
                const stylistData: Stylist = await stylistResponse.json();
                setStylist(stylistData);

                // fetch all services
                const servicesResponse = await fetch("http://localhost:5170/api/services");
                const servicesData: Service[] = await servicesResponse.json();
                
                // filter services to only those offered by this stylist
                const filteredServices = servicesData.filter(service => stylistData.serviceIds.includes(service.id.toString()));
                setServices(filteredServices);

            }catch (err){
                console.error('Error fetching stylist profile:', err);
                router.push("/stylists");
            }finally{
                setLoading(false);
            }
        }
        
        fetchStylistProfile();
    }, [])
    
    if (loading){
        return <p className={"p-8 text-gray-500"}>Loading Stylist...</p>;
    }
    if (!stylist){
        return <p className={"p-8 text-red-500"}>Stylist not found.</p>;
    }
    return(
        <div className="bg-gray-50 min-h-screen py-10">
            <div className={"p-8 max-w-3xl bg-white shadow rounded mx-auto"}>
                {/* Stylist Info */}
                <h1 className={"text-4xl font-extrabold mb-4 text-gray-800"}>{stylist.fullName}</h1>
                <p className={"mb-2 text-gray-600"}>📍 Location: {stylist.location}</p>
                <p className={"mb-2 text-gray-600"}>⭐ Rating: {stylist.rating} / 5</p>
                <p className={"mb-6 text-gray-600"}>Specialties: {stylist.specialties.join(", ")}</p>

                {/* Services */}
                <h2 className="text-2xl font-bold mb-4 text-gray-800">Services</h2>
                {services.length > 0 ? (
                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                        {services.map(service => (
                            <div
                                key={service.id}
                                className="border rounded-lg p-5 shadow-sm bg-white flex flex-col justify-between"
                            >
                                <div>
                                    <h3 className="font-bold text-lg mb-1">{service.name}</h3>
                                    <p className="text-sm text-gray-600 mb-2">{service.durationMinutes} mins</p>
                                    <p className="text-blue-600 font-semibold">£{service.price}</p>
                                </div>
                                <button
                                    className={"mt-4 bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 transition"}
                                    onClick={() => router.push(`/appointments?stylistId=${params.id}`)}
                                >
                                    Book Appointment
                                </button>
                                
                            </div>
                        ))}
                    </div>
                ) : (
                    <p className="text-gray-500">This stylist hasn't listed any services yet.</p>
                )}
                {/* Future: Embed Instagram or photos here */}
            </div>
        </div>
    );
}