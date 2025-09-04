



// const path = require('path');

// module.exports = {
//   entry: './ClientApp/src/index.jsx',
//   output: {
//     path: path.resolve(__dirname, 'wwwroot/js'),
//     filename: 'bundle.js',
//   },
//   resolve: {
//     extensions: ['.js', '.jsx'],
//   },
//   module: {
//     rules: [
//       {
//         test: /\.(js|jsx)$/,
//         exclude: /node_modules/,
//         use: 'babel-loader',
//       },
//       {
//         test: /\.css$/,              // 👈 Support for importing CSS files
//         use: ['style-loader', 'css-loader'],
//       },
//     ],
//   },
// };



const path = require('path');

module.exports = {
  entry: './ClientApp/src/index.jsx',
  output: {
    path: path.resolve(__dirname, 'wwwroot/js'),
    filename: 'bundle.js',
  },
  resolve: {
    extensions: ['.js', '.jsx', '.css'],  // added '.css'
  },
  module: {
    rules: [
      {
        test: /\.(js|jsx)$/,
        exclude: /node_modules/,
        use: 'babel-loader',
      },
      {
        test: /\.css$/,
        use: ['style-loader', 'css-loader'],
      },
    ],
  },
};
