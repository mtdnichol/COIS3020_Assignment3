﻿/*
 * Notes
 *  - Region QuadTree represents a partition of space by decomposing image into four equal quadrants
 *  - 
 */

using System;

namespace COIS3020_Assignment3 {
    public enum Color {BLACK, WHITE, GRAY}
    public class QuadTree {
        private Node root;
        private int length;
        
        // Public QuadTree
        // Build a compressed QuadTree given a 2^k x 2^k image, storing in root, then compresses
        // Parameters: image --> 2d array composed of Color enum representing an image, size --> length of the array
        // Note: Assume each entry of the matrix is either BLACK or WHITE.
        public QuadTree(Color[,] image, int size) {
            this.length = size;
            this.root = Construct(image, 0, 0, size);
            Compress();
            Console.WriteLine();
        }

        private QuadTree(Node node, int size) {
            this.root = node;
            this.length = size;
            Compress();
        }

        // Private Construct
        // Constructs a QuadTree node from a given 2d array representing an image
        // Returns a constructed QuadTree of size NxN
        // Note: Must be compressed after
        // Parameters: image --> 2d array composed of Color enum representing an image, x/y --> starting indices of a quadrant, n --> length of the array
        // x and y are starting indices of a quadrant, n is size
        // h = n/2
        //    NW quadrant = x, y    NE quadrant = x, y + h    SE quadrant = x + h, y + h    SW quadrant = x + h, y
        private Node Construct(Color[,] image, int x, int y, int n) {
            Node node = new Node();
            if (n == 1) {
                node.setColor(image[x,y]);
                return node;
            }
            node.setNW(Construct(image, x, y, n/2));
            node.setNE(Construct(image, x, y + n/2, n/2));
            node.setSE(Construct(image, x + n/2, y + n/2, n/2));
            node.setSW(Construct(image, x + n/2, y, n/2));

            return node;
        }
        
        // Public Print
        // Print the image represented by the current QuadTree
        // Parameters: None
        // Note: Use B for BLACK and W for WHITE. You can compare against the original image for testing
        public void Print() {
            Color[,] testArray = new Color[length,length];
            Print(root, testArray, 0, 0, length);
            
            //Print the array to the console
            for (int i = 0; i < length; i++) {
                for (int j = 0; j < length; j++) {
                    Console.Write(testArray[i,j] == Color.BLACK ? "B " : "W ");
                }
                Console.WriteLine();
            }
        }

        // Private Print
        // Constructs 2d array representing an image from a given QuadTree
        // Mutates a constructed Color[,] of size NxN
        // Parameters: node --> current QuadTree node, image --> 2d array composed of Color enum to be mutated, x/y --> starting indices of a quadrant, n --> length of the quadrant
        // h = n/2
        //    NW quadrant = x, y    NE quadrant = x, y + h    SE quadrant = x + h, y + h    SW quadrant = x + h, y
        private void Print(Node node, Color[,] testArray, int x, int y, int n) {
            switch (node.getColor()) { //Check the color of the current node
                case Color.WHITE: //If either black or white, all nodes beneath are that color
                case Color.BLACK:
                    for (int i = x; i < n + x; i++) //Iterate over the current section and copy it into the Color array
                        for (int j = y; j < n + y; j++) {
                            testArray[i, j] = node.getColor();
                        }
                    break;
                case Color.GRAY: //If gray, more nodes in the sub quadrants.  Traverse each
                    Print(node.getNW(), testArray, x, y, n / 2);
                    Print(node.getNE(), testArray, x, y + n / 2, n / 2);
                    Print(node.getSE(), testArray, x + n / 2, y + n / 2, n / 2);
                    Print(node.getSW(), testArray, x + n / 2, y, n / 2);
                    break;
                default:
                    Console.WriteLine("Error...");
                    break;
            }
        }
        
        // Public Print
        // Recursively compress the current QuadTree to minimize its number of nodes through private compress
        // Parameters: None
        public void Compress() {
            Compress(this.root);
        }

        // Private Compress
        // Recursively compress the current QuadTree to minimize its number of nodes
        // Parameters: node --> represents the current node to be compressed
        // Note: Makes many references to a LikeChildren method inside of the Node object
        private void Compress(Node node) {
            if (node.getColor() != Color.GRAY) //If the node colour isn't grey, a leaf node is reached and can no longer compress
                return;
            Compress(node.getNW()); //Otherwise, traverse each quadrant to compress each
            Compress(node.getNE());
            Compress(node.getSE());
            Compress(node.getSW());

            if (node.LikeChildren()) {//Check if the node has like children (same colors)
                node.setColor(node.getNW().getColor()); //If node has like children, the color of the node can be set to the color of its children
                node.ClearChildren();
            }
        }

        //Update and compress (if necessary) the current QuadTree when the color at index [i,j] of the original
        //    image is switched from BLACK to WHITE (or vice versa). Modify, but do not rebuild the QuadTree.
        //i,j are user passed coordinates, x,y are current section coordinates
        public void Switch(int i, int j) {
            Switch(i, j, 0,0, root, length);
        }

        // h = n/2
        //    NW quadrant = x, y    NE quadrant = x, y + h    SE quadrant = x + h, y + h    SW quadrant = x + h, y
        private void Switch(int i, int j, int x, int y, Node node, int size) {
            if (node == null) return;
            if (node.getColor() != Color.GRAY && x + size >= i && y + size >= j) { //Node is a leaf node, and within coordinates
                if (size == 1) {
                    if (i == x && j == y) {
                        node.setColor(node.getColor() == Color.BLACK ? Color.WHITE : Color.BLACK);
                    }
                } else { //Size is not 1, so node theoretically has children of all the same color.  Populate children
                    node.setNW(new Node(node.getColor()));
                    node.setNE(new Node(node.getColor()));
                    node.setSE(new Node(node.getColor()));
                    node.setSW(new Node(node.getColor()));
                    node.setColor(Color.GRAY);
                }
            }

            if (node.getColor() == Color.GRAY) { //Node has children, not a leaf node
                Switch(i, j, x, y, node.getNW(), size/2); //Call switch on each child
                Switch(i, j, x, y + size/2, node.getNE(), size/2);
                Switch(i, j, x + size/2, y + size/2, node.getSE(), size/2);
                Switch(i, j, x + size/2, y, node.getSW(), size/2);
                
                if (node.LikeChildren()) { //If the node has like children, compress on the way back up the tree
                    node.setColor(node.getNW().getColor());
                    node.ClearChildren();
                } else {
                    node.setColor(Color.GRAY); //If not like, traverse up tree
                }
            }
        }

        // Public Union
        // Create, compress, and return a QuadTree which is the union of a given QuadTree Q and the current root
        public QuadTree Union(QuadTree Q) {
            Node unionNode = Union(Q.getRoot(), root); //Creates a new root node by recursively unionizing the two trees
            QuadTree unionQuadTree = new QuadTree(unionNode, length); //Creates a new QuadTree with the completed root
            unionQuadTree.Compress(); //Compresses the new QuadTree

            return unionQuadTree; //Returns the resultant QuadTree to the caller
        }
        
        // Public Union
        // Create, compress, and return a QuadTree which is the union of a given QuadTree treeOne and the current QuadTree treeTwo
        // Parameters: Node treeOne --> Tree to be unionized with the root, treeTwo --> current root
        // Rules: Black if either image has a black in that location.  White only when both are white
        // Cases:
        //    1. t1 or t2 black, the corresponding node created is black.  If one black one gray, node contains a subtree underneath which need not be traversed
        //    2. t1 and t2 white, t2 and the subtree underneath is copied to created node
        //    3. Both t1 and t2 gray, corresponding children are considered
        private Node Union(Node treeOne, Node treeTwo) {
            Node node = new Node(); //New node is created (deep copy)
            
            if (treeOne.getColor() == Color.BLACK || treeTwo.getColor() == Color.BLACK) { //Either black
                node.setColor(Color.BLACK);
            } else if (treeOne.getColor() == Color.WHITE) { //Tree one is white, contents of tree two copied over to node
                node.setColor(treeTwo.getColor());
                node.setNW(treeTwo.getNW());
                node.setNE(treeTwo.getNE());
                node.setSE(treeTwo.getSE());
                node.setSW(treeTwo.getSW());
            } else if (treeTwo.getColor() == Color.WHITE) { //Tree two is white, contents of tree two copied over to node
                node.setColor(treeOne.getColor());
                node.setNW(treeOne.getNW());
                node.setNE(treeOne.getNE());
                node.setSE(treeOne.getSE());
                node.setSW(treeOne.getSW());
            } else { //Both gray
                node.setNW(Union(treeOne.getNW(), treeTwo.getNW())); //All children are considered, and traversed
                node.setNE(Union(treeOne.getNE(), treeTwo.getNE()));
                node.setSE(Union(treeOne.getSE(), treeTwo.getSE()));
                node.setSW(Union(treeOne.getSW(), treeTwo.getSW()));
            }

            return node; //Returns node after all paths have been traversed, and new tree is unionized
        }
        
        private Node getRoot() { return this.root; }
    }
}