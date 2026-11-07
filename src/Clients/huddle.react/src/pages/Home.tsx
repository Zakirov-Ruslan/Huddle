import idunno from "../img/huddle-mascot/idunno.png";

/**
 * Home screen: keeps the same two-column rhythm as a server page
 * (empty sidebar + content), but the sidebar is only shown from `md` up.
 */
function Home() {
    return (
        <div className="flex min-h-0 flex-1 flex-col md:flex-row">
            <aside className="hidden w-60 shrink-0 flex-col border-gray-300 bg-gray-100 md:flex md:w-60 md:border-r lg:w-[300px]">
                <div className="flex h-[50px] shrink-0 items-center border-b border-gray-300 px-5 text-lg font-semibold text-gray-700">
                    Home
                </div>
                <div className="min-h-0 flex-1 overflow-y-auto" />
            </aside>

            <section className="flex min-h-0 min-w-0 flex-1 flex-col">
                <header className="flex h-[50px] shrink-0 items-center border-b border-gray-300 bg-white px-4 font-semibold text-gray-700">
                    <span className="md:hidden">Home</span>
                </header>

                <div className="flex min-h-0 flex-1 flex-col items-center justify-center overflow-y-auto bg-white p-6 select-none">
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
                        className="h-48 w-48 md:h-60 md:w-60"
                    />
                </div>
            </section>
        </div>
    );
}

export default Home;