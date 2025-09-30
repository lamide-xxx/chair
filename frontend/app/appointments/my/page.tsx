"use client"
import {useEffect, useState} from "react";

interface Appointment {
    id: number;
    userId: string;
    service: Service;
    stylist: Stylist;
    startTime: string;
    endTime: string;
    status: number;
}

interface Service {
    id: number;
    name: string;
}

interface Stylist {
    id: number;
    fullName: string;
}

export default function MyAppointmentsPage(){
    const [appointments, setAppointments] = useState<Appointment[]>([]);
    const [services, setServices] = useState<Service[]>([]);
    const [stylists, setStylists] = useState<Stylist[]>([]);
    const [loading, setLoading] = useState(true);

    // hardcoded test user
    const userId = "7621f8c8-9275-4ded-82cb-cf117adf0e75";
    
    useEffect(() => {
        async function fetchData(){
            try{
                const [appointmentsResponse, servicesResponse, stylistsResponse] = await Promise.all([
                    fetch(`http://localhost:5170/api/appointments/user/${userId}`),
                    fetch("http://localhost:5170/api/services/"),
                    fetch("http://localhost:5170/api/stylists/")
                ]);
                
                if (!appointmentsResponse.ok) {
                    throw new Error(`HTTP error fetching appointments! status: ${appointmentsResponse.status}; message: ${appointmentsResponse.statusText}, body: ${await appointmentsResponse.text()}`);
                }
                if (!servicesResponse.ok) {
                    throw new Error(`HTTP error fetching services! status: ${servicesResponse.status}; message: ${servicesResponse.statusText}, body: ${await servicesResponse.text()}`);
                }  
                if (!stylistsResponse.ok) {
                    throw new Error(`HTTP error fetching stylists! status: ${stylistsResponse.status}; message: ${stylistsResponse.statusText}, body: ${await stylistsResponse.text()}`);
                }
                const [appointmentsData, servicesData, stylistsData] = await Promise.all([
                    appointmentsResponse.json(),
                    servicesResponse.json(),
                    stylistsResponse.json()
                ]);
                
                setAppointments(appointmentsData);
                setServices(servicesData);
                setStylists(stylistsData);
            }catch (err){
                console.error('Error fetching appointments:', err);
            }finally {
                setLoading(false);
            }
        }
        
        fetchData();
        
    }, [])

    function getServiceName(id: number) {
        return services.find((s) => s.id === id)?.name || "Unknown Service";
    }

    function getStylistName(id: number) {
        return stylists.find((s) => s.id === id)?.fullName || "Unknown Stylist";
    }

    function getStatusText(status: number) {
        switch (status) {
            case 0: return "Pending";
            case 1: return "Confirmed";
            case 2: return "Completed";
            case 3: return "Cancelled";
            default: return "Unknown";
        }
    }
    
    if (loading){
        return <p className={"p-8 text-gray-500"}>Loading Appointments...</p>;
    }
    return (
        <div className={"p-8 max-w-3xl mx-auto"}>
            <h1 className="text-3xl font-bold mb-6">My Appointments</h1>
            {appointments.length === 0 ? (
                <p className="text-gray-500">You have no appointments yet.</p>
            ) : (
                <ul className="space-y-4">
                    {appointments.map((appt) => (
                        <li
                            key={appt.id}
                            className="border rounded-lg p-4 shadow-sm bg-white"
                        >
                            <h2 className="text-lg font-semibold">
                                {getServiceName(appt.service.id)}
                            </h2>
                            <p className="text-gray-600">
                                Stylist: {getStylistName(appt.stylist.id)}
                            </p>
                            <p className="text-gray-600">
                                Time: {new Date(appt.startTime).toLocaleString()}
                            </p>
                            <p className="text-gray-500">Status: {getStatusText(appt.status)}</p>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    )
}