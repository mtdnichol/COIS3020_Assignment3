/*
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
            Console.WriteLine();
            Compress();
        }

        public QuadTree(Node node, int size) {
            this.root = node;
            this.length = size;
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

            Console.WriteLine();
            //Print the array to the console
            for (int i = 0; i < length; ++i) {
                for (int j = 0; j < length; ++j) {
                    if (testArray[i,j] == Color.BLACK)
                        Console.Write("B");
                    else
                        Console.Write("W");
                }
                Console.WriteLine();
            }
        }

        // Private Print
        // Constructs 2d array representing an image from a given QuadTree
        // Mutates a constructed Color[,] of size NxN
        // Parameters: node --> current QuadTree node, image --> 2d array composed of Color enum to be mutated, x/y --> starting indices of a quadrant, n --> length of the quadrant
        private void Print(Node node, Color[,] testArray, int x, int y, int n) {
            switch (node.getColor()) {
                case Color.WHITE:
                case Color.BLACK:
                    for (int i = x; i < n/2; i++)
                        for (int j = y; j < n/2; ++j) {
                            testArray[x, y] = node.getColor();
                        }
                    break;
                case Color.GRAY:
                    Print(node.getNW(), testArray, x, y, n / 2);
                    Print(node.getNE(), testArray, x, y + n / 2, n / 2);
                    Print(node.getSE(), testArray, x + n / 2, y + n / 2, n / 2);
                    Print(node.getSW(), testArray, x + n / 2, y, n / 2);
                    break;
            }
        }

        //Recursively compress the current QuadTree to minimize its number of nodes.
        public void Compress() {
            Compress(this.root);
        }

        private void Compress(Node node) {
            if (node.getColor() != Color.GRAY)
                return;
            Compress(node.getNW());
            Compress(node.getNE());
            Compress(node.getSE());
            Compress(node.getSW());
            
            if (node.LikeChildren())
                node.setColor(node.getNW().getColor()); //TEST
            else {
                node.setColor(Color.GRAY);
            }
        }

        //Update and compress (if necessary) the current QuadTree when the color at index [i,j] of the original
        //    image is switched from BLACK to WHITE (or vice versa). Modify, but do not rebuild the quadtree.
        public void Switch(int i, int j) {
            
        }

        private void Switch() {
            
        }

        public QuadTree Union(QuadTree Q) {
            Node unionNode = Union(Q.getRoot(), root);
            QuadTree unionQuadTree = new QuadTree(unionNode, length);
            unionQuadTree.Compress();

            return unionQuadTree;

            //return new QuadTree(Union(Q.getRoot(), root), this.length);
        }

        // Rules: Black if either image has a black in that location.  White only when both are white
        // Cases:
        //    1. t1 or t2 black, the corresponding node created is black.  If one black one gray, node contains a subtree underneath which need not be traversed
        //    2. t1 and t2 white, t2 and the subtree underneath is copied to created node
        //    3. Both t1 and t2 gray, corresponding children are considered
        private Node Union(Node treeOne, Node treeTwo) {
            Node node = new Node();
            
            if (treeOne.getColor() == Color.BLACK || treeTwo.getColor() == Color.BLACK) { //Either black
                node.setColor(Color.BLACK);
            } else if (treeOne.getColor() == Color.WHITE && treeTwo.getColor() == Color.WHITE) { //Both white
                node.setColor(Color.WHITE);
            } else { //Both gray
                node.setNW(Union(treeOne.getNW(), treeTwo.getNW()));
                node.setNE(Union(treeOne.getNE(), treeTwo.getNE()));
                node.setSE(Union(treeOne.getSE(), treeTwo.getSE()));
                node.setSW(Union(treeOne.getSW(), treeTwo.getSW()));
            }

            return node;
        }

        public int getLength() { return this.length; }
        private Node getRoot() { return this.root; }
    }
}