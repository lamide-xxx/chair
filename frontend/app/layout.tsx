import "./globals.css";
import {Metadata} from "next";
import Header from "@/app/components/Header";


export const metadata : Metadata = {
  title: "Chair",
  description: "Book your Stylist with ease",
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html lang="en">
        <body className="min-h-screen flex flex-col">
            <Header />
            <main className={"flex-grow"}>{children}</main>
        </body>
    </html>
  );
}
