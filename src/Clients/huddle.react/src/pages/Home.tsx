import { FaUserFriends } from "react-icons/fa";
import { BiMessageAdd } from "react-icons/bi";

function Home() {

    return (
        <>
            <div className="border-b-1 border-gray-300"></div>
            <div className="border-b-1 border-gray-300 bg-white"></div>
            <div>
                <nav className="flex-1 overflow-y-auto p-4">
                    <ul className="space-y-2">
                        <li>
                            <button type="button" className="flex w-full items-center gap-2 rounded-xl px-3 py-2 text-left transition-colors" >
                                <FaUserFriends className="" />
                                <span className="flex-grow-1">Friends</span>
                            </button>
                        </li>
                        <li>
                            <button type="button" className="flex w-full items-center gap-2 rounded-xl px-3 py-2 text-left transition-colors" >
                                <BiMessageAdd className="" />
                                <span className="flex-grow-1">Invites</span>
                            </button>
                        </li>
                    </ul>
                </nav>
            </div>

            <div className="bg-white">
            </div>
        </>
    );
}

export default Home;