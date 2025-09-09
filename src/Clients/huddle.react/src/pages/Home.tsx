import { FaUserFriends } from "react-icons/fa";
import { BiMessageAdd } from "react-icons/bi";
import idunno from "../img/huddle-mascot/idunno.png";

function Home() {

    return (
        <>
            <div className="flex items-center justify-center border-b-1 border-gray-300 font-medium">Home</div>
            <div className="border-b-1 border-gray-300 bg-white"></div>
            <div>

            </div>

            <div className="flex flex-col items-center justify-center bg-white select-none">
               

                        <div className="relative max-w-md rotate-1 transform rounded-3xl bg-white p-8 shadow">
                            <div className="text-center">
                                <h2 className="mb-4 text-3xl font-bold tracking-wide text-gray-800">Hello!</h2>
                                <p className="text-lg leading-relaxed text-gray-700">
                                    Its just a MVC project of text and voice chat.
                                    You can create a server using plus button on left navigation panel and try to chat
                                </p>
                            </div>
                        </div>


                <img
                    src={idunno}
                    alt="empty content"
                    className="h-60 w-60"
                />
            </div>
        </>
    );
}

export default Home;