import sys

if sys.version_info[0] == 2:

    import SimpleHTTPServer
    import SocketServer

    PORT = 8000

    class Handler(SimpleHTTPServer.SimpleHTTPRequestHandler):
        pass

    Handler.extensions_map['.wasm'] = 'application/wasm'

    httpd = SocketServer.TCPServer(("", PORT), Handler)

    print ("python 2 serving at port", PORT)
    httpd.serve_forever()


if sys.version_info[0] == 3:
    
    if sys.version_info[1] < 8: # this checks for minor version smaller than 8
        print('[Error] python 3 version needs to be higher than 3.8 in order to serve the right mime type for wasm (application/wasm)')
        print("Your version: " + str(sys.version))
        input("Press enter to exit...")
        exit(self, 1)

    import http.server
    import socketserver

    PORT = 8000

    Handler = http.server.SimpleHTTPRequestHandler
    Handler.extensions_map['.wasm'] = 'application/wasm'
    Handler.extensions_map['.js'] = 'text/javascript'

    with socketserver.TCPServer(("", PORT), Handler) as httpd:
        print("python 3 serving at port", PORT)
        httpd.serve_forever()

