# Json Bridge

This is a package for working with a local JSON-over-TCP server.

## Usage

Look at the 'tests' folder for an example scene.

Typically you would extend `NetworkManagerBase` and then add an instance of it 
to the scene.

Notice that connections are managed in a separate thread, so this will only
work on platforms that support threads. 

Additionally the hot-reload will cause threads to hang, so disable hot-reload
when working with this library.

And example server can be found at: 

https://github.com/shadowmint/go-transport/tree/master/tests

## Install

From your unity project folder:

    npm init
    npm install shadowmint/unity-n-jsonbridge --save
    echo Assets/packages >> .gitignore
    echo Assets/packages.meta >> .gitignore

The package and all its dependencies will be installed in
your Assets/packages folder.

## Development

Setup and run tests:

    npm install
    npm install ..
    cd test
    npm install
    gulp

Remember that changes made to the test folder are not saved to the package
unless they are copied back into the source folder.

To reinstall the files from the src folder, run `npm install ..` again.

### Tests

All tests are wrapped in `#if ...` blocks to prevent test spam.

You can enable tests in: Player settings > Other Settings > Scripting Define Symbols

The test key for this package is: N_JSONBRIDGE_TESTS
