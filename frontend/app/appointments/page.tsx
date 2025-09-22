"use client";
import React, { useEffect, useState } from "react";

interface Stylist {id: number; fullName: string;}
interface Service {id: number; name: string; date: string; price: string;}

export default function AppointmentsPage(){
    const [services, setServices] = useState<Service[]>([]);
    const [stylists, setStylists] = useState<Stylist[]>([]);
    const [serviceId, setServiceId] = useState<string>("");
    const [stylistId, setStylistId] = useState<string>("");
    const [date, setDate] = useState<string>("");
    const [loading, setLoading] = useState(true);
    
    useEffect(() =>{
        async function fetchData(){
            try{
                const servicesResponse = await fetch("http://localhost:5170/api/services/");
                const servicesData: Service[] = await servicesResponse.json();
                setServices(servicesData);
                
                const stylistsResponse = await fetch("http://localhost:5170/api/stylists/");
                const stylistsData: Stylist[] = await stylistsResponse.json();
                setStylists(stylistsData);
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
            const response = await fetch("http://localhost:5170/api/appointments/", {
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
            alert("Appointment created successfully!");
        }catch (err){
            console.error('Error creating appointment:', err);
            alert("Error creating appointment");
        }
    }
    
    if(loading){
        return <p>Loading Appointments...</p>;
    }
    return (
        <div className={"p-8"}>
            <h1 className={"text-3xl font-bold mb-6"}>Book an Appointment</h1>
            <form onSubmit={handleSubmit} className={"space-y-4"}>
                <div>
                    <label
                        htmlFor="services"
                        className={"block mb-1"}
                    >
                        Service</label>
                    <select
                        id="services"
                        className={"border p-2 rounded w-full"}
                        value={serviceId}
                        onChange={(e)=>setServiceId(e.target.value)}
                    >
                        <option>Select a Service</option>
                        {services.map((service: Service) =>(
                            <option key={service.id} value={service.id}>{service.name}</option>
                            )
                        )}
                    </select>
                </div>

                <div>
                    <label
                        htmlFor="stylists"
                        className={"block"}
                    >
                        Stylist</label>
                    <select
                        id="stylists"
                        className={"border p-2 rounded w-full"}
                        value={stylistId}
                        onChange={(e)=>setStylistId(e.target.value)}
                    >
                        <option>Select a Stylist</option>
                        {stylists.map((stylist: Stylist) =>(
                                <option key={stylist.id} value={stylist.id}>{stylist.fullName}</option>
                            )
                        )}
                    </select>
                </div>

                <div>
                    <label htmlFor="date" className={"block mb-1"}>Date & Time</label>
                    <input
                        id={"date"}
                        type="datetime-local"
                        className={"border p-2 rounded w-full"}
                        value={date}
                        onChange={(e)=>setDate(e.target.value)}
                        />
                </div>
                
                <button type="submit" className={"bg-blue-500 text-white px-4 py-2 rounded"}>
                    Book
                </button>
            </form>
        </div>
    );
}