const path = require('path');
const MiniCssExtractPlugin = require("mini-css-extract-plugin"); 

module.exports = {
    mode: "production",
    entry: path.resolve(__dirname, './gantt/src/index.tsx'),
    output: {
        path: path.resolve(__dirname, "./wwwroot/js"),
        filename: "gantt.bundle.js",
        library: { type: "module" }
    },
    experiments: { outputModule: true },
    module: {
        rules: [
            {
                test: /\.(ts|tsx)$/,
                exclude: /node_modules/,
                use: 'ts-loader'
            },
            {
                test: /\.css$/,
                use: [
                    MiniCssExtractPlugin.loader, 
                    'css-loader'
                ]
            }
        ]
    },
    resolve: {
        extensions: ['.tsx', '.ts', '.js']
    }, 
    plugins: [
        new MiniCssExtractPlugin({
            filename: "gantt.bundle.css"
        }),
    ],
};