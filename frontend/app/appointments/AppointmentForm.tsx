"use client";

import React, {useEffect, useState} from "react";
import {useRouter, useSearchParams} from "next/navigation";


interface Stylist {id: number; fullName: string;}
interface Service {id: number; name: string; duration: number; price: string;}

export default function AppointmentForm(){
    const apiBaseUrl = process.env.NEXT_PUBLIC_API_BASE_URL;

    const router = useRouter();
    const searchParams = useSearchParams();

    const queryStylistId = searchParams.get("stylistId") || "";
    const queryServiceId = searchParams.get("serviceId") || "";

    const [services, setServices] = useState<Service[]>([]);
    const [stylists, setStylists] = useState<Stylist[]>([]);
    const [serviceId, setServiceId] = useState<string>(queryServiceId);
    const [stylistId, setStylistId] = useState<string>(queryStylistId);
    const [date, setDate] = useState<string>("");
    const [loading, setLoading] = useState(true);

    useEffect(() =>{
        async function fetchData(){
            try{
                const servicesResponse = await fetch(`${apiBaseUrl}/api/services/`);
                setServices(await servicesResponse.json());

                const stylistsResponse = await fetch(`${apiBaseUrl}/api/stylists/`);
                setStylists(await stylistsResponse.json());
            }catch (err){
                console.error('Error fetching data:', err);
            }finally {
                setLoading(false);
            }
        }

        fetchData();
    }, []);


    async function handleSubmit(e: React.FormEvent){
        e.preventDefault();
        try{
            const response = await fetch(`${apiBaseUrl}/api/appointments/`, {
                method: "POST",
                headers: {"Content-Type": "application/json"},
                body: JSON.stringify({
                    userId: "7621f8c8-9275-4ded-82cb-cf117adf0e75", // temporary test user
                    serviceId: serviceId,
                    stylistId: stylistId,
                    startTime: date,
                    endTime: date,
                    status: 0
                }),
            });

            if (!response.ok) {
                throw new Error(`HTTP error creating appointment! status: ${response.status}; message: ${response.statusText}, body: ${await response.text()}`);
            }

            router.push("/appointments/success");
        }catch (err){
            console.error('Error creating appointment:', err);
            alert("Error creating appointment");
        }
    }
    
    if (loading){
        return <p className="p-8 text-gray-500">Loading Appointments...</p>;
    }
    
    return(
        <div className={"bg-gray-50 min-h-screen py-12 px-4"}>
            <div className={"max-w-lg mx-auto bg-white shadow rounded-lg p-8"}>
                <h1 className={"text-3xl font-extrabold mb-6 text-gray-800"}>Book an Appointment</h1>
                <form onSubmit={handleSubmit} className={"space-y-5"}>
                    <div>
                        <label
                            htmlFor="services"
                            className={"block text-gray-700 font-semibold mb-2"}
                        >
                            Service</label>
                        <select
                            id="services"
                            className={"w-full border p-2 rounded-lg"}
                            value={serviceId}
                            onChange={(e)=>setServiceId(e.target.value)}
                        >
                            <option value={""}>Select a Service</option>
                            {services.map((service: Service) =>(
                                    <option key={service.id} value={service.id}>
                                        {service.name} - £{service.price}
                                    </option>
                                )
                            )}
                        </select>
                    </div>
    
                    <div>
                        <label
                            htmlFor="stylists"
                            className={"block text-gray-700 font-semibold mb-2"}
                        >
                            Stylist</label>
                        <select
                            id="stylists"
                            className={"border p-2 rounded-lg w-full"}
                            value={stylistId}
                            onChange={(e)=>setStylistId(e.target.value)}
                        >
                            <option value={""}>Select a Stylist</option>
                            {stylists.map((stylist: Stylist) =>(
                                    <option key={stylist.id} value={stylist.id}>
                                        {stylist.fullName}
                                    </option>
                                )
                            )}
                        </select>
                    </div>
    
                    <div>
                        <label htmlFor="date" className={"block text-gray-700 font-semibold mb-2"}>Date & Time</label>
                        <input
                            id={"date"}
                            type="datetime-local"
                            className={"border p-2 rounded-lg w-full"}
                            value={date}
                            onChange={(e)=>setDate(e.target.value)}
                        />
                    </div>
    
                    <button type="submit" className={"w-full bg-blue-500 text-white px-4 py-3 rounded-lg font-semibold hover:bg-blue-600 transitio"}>
                        Confirm Booking
                    </button>
                </form>
            </div>
        </div>
    )
}